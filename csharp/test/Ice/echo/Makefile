# **********************************************************************
#
# Copyright (c) 2003-2016 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

top_srcdir	= ../../..

TARGETS		= server.exe

S_SRCS		= Server.cs BlobjectI.cs

SLICE_SRCS	= $(SDIR)/Test.ice

SDIR		= .

GDIR		= generated

include $(top_srcdir)/config/Make.rules.cs

MCSFLAGS	:= $(MCSFLAGS) -target:exe

SLICE2CSFLAGS	:= $(SLICE2CSFLAGS) -I. -I$(slicedir)

server.exe: $(S_SRCS) $(GEN_SRCS)
	$(MCS) $(MCSFLAGS) -out:$@ $(call ref,Ice) $(subst /,$(DSEP),$^)
