# **********************************************************************
#
# Copyright (c) 2003-2015 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

top_srcdir	= ..\..\..

!if "$(WINRT)" != "yes"
NAME_PREFIX	=
EXT		= .exe
!else
NAME_PREFIX	= Ice_invoke_
EXT		= .dll
!endif

SERVER		= $(NAME_PREFIX)server

TARGETS		= $(SERVER)$(EXT)

SLICE_OBJS	= .\Test.obj

OBJS		= $(SLICE_OBJS) \
		  .\BlobjectI.obj \
		  .\Server.obj

!include $(top_srcdir)/config/Make.rules.mak

CPPFLAGS	= -I. -I../../include $(CPPFLAGS) -DWIN32_LEAN_AND_MEAN

!if "$(WINRT)" != "yes"
LD_TESTFLAGS	= $(LD_EXEFLAGS) $(SETARGV)
!else
LD_TESTFLAGS	= $(LD_DLLFLAGS) /export:dllMain
!endif

!if "$(GENERATE_PDB)" == "yes"
CPDBFLAGS        = /pdb:$(CLIENT).pdb
SPDBFLAGS        = /pdb:$(SERVER).pdb
!endif

$(SERVER)$(EXT): $(OBJS)
	$(LINK) $(LD_TESTFLAGS) $(SPDBFLAGS) $(OBJS) $(PREOUT)$@ $(PRELIBS)$(LIBS)
	@if exist $@.manifest echo ^ ^ ^ Embedding manifest using $(MT) && \
	    $(MT) -nologo -manifest $@.manifest -outputresource:$@;#1 && del /q $@.manifest

clean::
	del /q Test.cpp Test.h
