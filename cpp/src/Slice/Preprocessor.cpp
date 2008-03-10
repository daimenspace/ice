// **********************************************************************
//
// Copyright (c) 2003-2008 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

#include <IceUtil/DisableWarnings.h>
#include <Slice/Preprocessor.h>
#include <Slice/Util.h>
#include <Slice/SignalHandler.h>
#include <IceUtil/StringUtil.h>
#include <IceUtil/UUID.h>
#include <IceUtil/Unicode.h>
#include <algorithm>
#include <fstream>
#include <sys/types.h>
#include <sys/stat.h>
#include <cstring>

#ifndef _WIN32
#   include <sys/wait.h>
#endif

using namespace std;
using namespace Slice;

//
// Callback for Crtl-C signal handling
//
static Preprocessor* _preprocess = 0;

static void closeCallback()
{
    if(_preprocess != 0)
    {
        _preprocess->close();
    }
}


//
// mcpp defines
//
namespace Slice
{

enum Outdest
{
    Out=0, Err=1, Dbg=2, Num_Outdest=3
}; 

};

extern "C" int   mcpp_lib_main(int argc, char** argv);
extern "C" void  mcpp_use_mem_buffers(int tf);
extern "C" char* mcpp_get_mem_buffer(Outdest od);

Slice::Preprocessor::Preprocessor(const string& path, const string& fileName, const vector<string>& args) :
    _path(path),
    _fileName(fileName),
    _args(args),
    _cppHandle(0)
{
    _preprocess = this;
    SignalHandler::setCallback(closeCallback);
}

Slice::Preprocessor::~Preprocessor()
{
    _preprocess = 0;
    SignalHandler::setCallback(0);

    close();
}

string
Slice::Preprocessor::getBaseName()
{
    string base(_fileName);
    string suffix;
    string::size_type pos = base.rfind('.');
    if(pos != string::npos)
    {
        base.erase(pos);
    }
    return base;
}

string
Slice::Preprocessor::addQuotes(const string& arg)
{
    //
    // Add quotes around the given argument to ensure that arguments
    // with spaces will be preserved as a single argument. We also
    // escape the "\" character to ensure that we don't end up with a
    // \" at the end of the string.
    //
    return "\"" + IceUtilInternal::escapeString(arg, "\\") + "\"";
}

string
Slice::Preprocessor::normalizeIncludePath(const string& path)
{
    string result = path;

    replace(result.begin(), result.end(), '\\', '/');

    string::size_type pos;
    while((pos = result.find("//")) != string::npos)
    {
        result.replace(pos, 2, "/");
    }

    if(result == "/" || (result.size() == 3 && isalpha(result[0]) && result[1] == ':' && result[2] == '/'))
    {
	return result;
    }
    
    if(result.size() > 1 && result[result.size() - 1] == '/')
    {
	result.erase(result.size() - 1);
    }

    return result;
}

FILE*
Slice::Preprocessor::preprocess(bool keepComments)
{
    if(!checkInputFile())
    {
        return 0;
    }

    //
    // Build arguments list.
    //
    vector<string> args = _args;
    if(keepComments)
    {
        args.push_back("-C");
    }
    args.push_back(_fileName);

    const char** argv = new const char*[args.size() + 1];
    argv[0] = "mcpp";
    for(unsigned int i = 0; i < args.size(); ++i)
    {
        argv[i + 1] = args[i].c_str();
    }

    //
    // Call mcpp using memory buffer.
    //
    mcpp_use_mem_buffers(1);
    mcpp_lib_main(static_cast<int>(args.size()) + 1, const_cast<char**>(argv));
    delete[] argv;

    //
    // Write output to temporary file. Print errors to stderr.
    //
    string result;
    char* buf = mcpp_get_mem_buffer(Out);

    _cppFile = ".preprocess." + IceUtil::generateUUID();
    SignalHandler::addFile(_cppFile);
#ifdef _WIN32
    _cppHandle = ::_wfopen(IceUtil::stringToWstring(_cppFile).c_str(), IceUtil::stringToWstring("w+").c_str());
#else
    _cppHandle = ::fopen(_cppFile.c_str(), "w+");
#endif
    if(buf)
    {
        ::fwrite(buf, strlen(buf), 1, _cppHandle);
    }
    ::rewind(_cppHandle);

    char* err = mcpp_get_mem_buffer(Err);
    if(err)
    {
        ::fputs(err, stderr);
    }

    mcpp_use_mem_buffers(0);

    return _cppHandle;
}

void
Slice::Preprocessor::printMakefileDependencies(Language lang, const vector<string>& includePaths)
{
    if(!checkInputFile())
    {
        return;
    }

    //
    // Build arguments list.
    //
    vector<string> args = _args;
    args.push_back("-M");
    args.push_back(_fileName);

    const char** argv = new const char*[args.size() + 1];
    for(unsigned int i = 0; i < args.size(); ++i)
    {
        argv[i + 1] = args[i].c_str();
    }
    
    //
    // Call mcpp using memory buffer.
    //
    mcpp_use_mem_buffers(1);
    mcpp_lib_main(static_cast<int>(args.size() + 1), const_cast<char**>(argv));
    delete[] argv;

    //
    // Get mcpp output/errors.
    //
    string unprocessed;
    char* buf = mcpp_get_mem_buffer(Out);
    if(buf)
    {
        unprocessed = string(buf);
    }

    char* err = mcpp_get_mem_buffer(Err);
    if(err)
    {
        ::fputs(err, stderr);
    }
    mcpp_use_mem_buffers(0);

    //
    // We now need to massage then result to get desire output.
    // First make it a single line.
    //
    string::size_type pos;
    while((pos = unprocessed.find("\\\n")) != string::npos)
    {
        unprocessed.replace(pos, 2, "");
    }

    //
    // Get the main output file name.
    //
#ifdef _WIN32
     string suffix = ".obj:";
#else
     string suffix = ".o:";
#endif
    pos = unprocessed.find(suffix) + suffix.size();
    string result = unprocessed.substr(0, pos);

    //
    // Process each dependency.
    //
    string::size_type end;
    while((end = unprocessed.find(".ice", pos)) != string::npos)
    {
        end += 4;
        string file = IceUtilInternal::trim(unprocessed.substr(pos, end - pos));
        
        //
        // Normalize paths if not relative path.
        //
        if(isAbsolute(file))
        {
            string newFile = file;
            string cwd = getCwd();
            for(vector<string>::const_iterator p = includePaths.begin(); p != includePaths.end(); ++p)
            {
                string includePath = *p;
                if(!isAbsolute(includePath))
                {
                    includePath = cwd + "/" + includePath;
                }
                includePath = normalizePath(includePath, false);

                if(file.compare(0, includePath.length(), includePath) == 0)
                {
                    string s = *p + file.substr(includePath.length());
                    if(isAbsolute(newFile) || s.size() < newFile.size())
                    {
                        newFile = s;
                    }
                }
            }
            file = newFile;
        }

        //
        // Escape spaces in the file name.
        //
        string::size_type space = 0;
        while((space = file.find(" ", space)) != string::npos)
        {
            file.replace(space, 1, "\\ ");
            space += 2;
        }

        //
        // Add to result
        //
        result += " \\\n " + file;
        pos = end;
    }
    result += "\n";

    /*
     * icecpp emits dependencies in any of the following formats, depending on the
     * length of the filenames:
     *
     * x.o[bj]: /path/x.ice /path/y.ice
     *
     * x.o[bj]: /path/x.ice \ 
     *  /path/y.ice
     *
     * x.o[bj]: /path/x.ice /path/y.ice \ 
     *  /path/z.ice
     *
     * x.o[bj]: \ 
     *  /path/x.ice
     *
     * x.o[bj]: \ 
     *  /path/x.ice \ 
     *  /path/y.ice
     *
     * Spaces embedded within filenames are escaped with a backslash. Note that
     * Windows filenames may contain colons.
     *
     */
    switch(lang)
    {
        case CPlusPlus:
        {
            //
            // Change .o[bj] suffix to .cpp suffix.
            //
            string::size_type pos;
            while((pos = result.find(suffix)) != string::npos)
            {
                result.replace(pos, suffix.size() - 1, ".cpp");
            }
            break;
        }
        case Java:
        {
            //
            // We want to shift the files left one position, so that
            // "x.cpp: x.ice y.ice" becomes "x.ice: y.ice".
            //

            //
            // Remove the first file.
            //
            string::size_type start = result.find(suffix);
            assert(start != string::npos);
            start = result.find_first_not_of(" \t\r\n\\", start + suffix.size()); // Skip to beginning of next file.
            assert(start != string::npos);
            result.erase(0, start);

            //
            // Find end of next file.
            //
            string::size_type pos = 0;
            while((pos = result.find_first_of(" :\t\r\n\\", pos + 1)) != string::npos)
            {
                if(result[pos] == ':')
                {
                    result.insert(pos, 1, '\\'); // Escape colons.
                    ++pos;
                }
                else if(result[pos] == '\\') // Ignore escaped characters.
                {
                    ++pos;
                }
                else
                {
                    break;
                }
            }

            if(pos == string::npos)
            {
                result.append(":");
            }
            else
            {
                result.insert(pos, 1, ':');
            }
            break;
        }
        case CSharp:
        {
            //
            // Change .o[bj] suffix to .cs suffix.
            //
            string::size_type pos;
            while((pos = result.find(suffix)) != string::npos)
            {
                result.replace(pos, suffix.size() - 1, ".cs");
            }
            break;
        }
        default:
        {
            abort();
            break;
        }
    }

    //
    // Output result
    //
    fputs(result.c_str(), stdout);
}

bool
Slice::Preprocessor::close()
{
    if(_cppHandle != 0)
    {
        int status = fclose(_cppHandle);
        _cppHandle = 0;

        if(status != 0)
        {
            return false;
        }

#ifdef _WIN32
        _unlink(_cppFile.c_str());
#else
        unlink(_cppFile.c_str());
#endif
    }
    
    return true;
}

bool
Slice::Preprocessor::checkInputFile()
{
    string base(_fileName);
    string suffix;
    string::size_type pos = base.rfind('.');
    if(pos != string::npos)
    {
        suffix = base.substr(pos);
        transform(suffix.begin(), suffix.end(), suffix.begin(), ::tolower);
    }
    if(suffix != ".ice")
    {
        cerr << _path << ": input files must end with `.ice'" << endl;
        return false;
    }
    
    ifstream test(_fileName.c_str());
    if(!test)
    {
        cerr << _path << ": can't open `" << _fileName << "' for reading" << endl;
        return false;
    }
    test.close();

    return true;
}
