// **********************************************************************
//
// Copyright (c) 2003-2015 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

#include <Client.h>
#include <StringConverterI.h>

using namespace std;
using namespace Demo;

int
main(int argc, char* argv[])
{
    int status = EXIT_SUCCESS;
    try
    {
        //
        // Set the process string converter and then initialize the
        // aplication.
        //
        IceUtil::setProcessStringConverter(new StringConverterI());
        Client app;
        status = app.main(argc, argv, "config.client");
    }
    catch(const Ice::Exception& ex)
    {
        cerr << ex << endl;
        status = EXIT_FAILURE;
    }
    return status;
}
