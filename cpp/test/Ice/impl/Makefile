# **********************************************************************
#
# Copyright (c) 2003-2016 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

top_srcdir	= ../../..

SERVER		= $(call mktestname,server)
SERVERAMD	= $(call mktestname,serveramd)

TARGETS		= $(SERVER) $(SERVERAMD)

SLICE_OBJS	= Test.o TestAMD.o

SOBJS		= Test.o \
		  TestI.o \
		  Server.o \

SAMDOBJS	= TestAMD.o \
		  TestAMDI.o \
		  ServerAMD.o

OBJS		= $(SOBJS) \
		  $(SAMDOBJS)

include $(top_srcdir)/config/Make.rules

CPPFLAGS	:= -I. -I../../include $(CPPFLAGS)

ifeq ($(CPP11_MAPPING),yes)
SLICE2CPPFLAGS	:= --impl-c++11 $(SLICE2CPPFLAGS)
else
SLICE2CPPFLAGS	:= --impl-c++98 $(SLICE2CPPFLAGS)
endif

Test.cpp: Test.ice $(SLICE2CPP)
	rm -f $(*F).h $(*F).cpp $(*F)I.h $(*F)I.cpp
	$(SLICE2CPP) $(SLICE2CPPFLAGS) $(*F).ice
	@touch $(*F).cpp

Test.h TestI.cpp TestI.h: Test.cpp
	

TestAMD.cpp: TestAMD.ice $(SLICE2CPP)
	rm -f $(*F).h $(*F).cpp $(*F)I.h $(*F)I.cpp
	$(SLICE2CPP) $(SLICE2CPPFLAGS) $(*F).ice
	@touch $(*F).cpp

TestAMD.h TestAMDI.cpp TestAMDI.h: TestAMD.cpp
	

$(SERVER): $(SOBJS)
	rm -f $@
	$(call mktest,$@,$(SOBJS),$(TEST_LIBS))

$(SERVERAMD): $(SAMDOBJS)
	rm -f $@
	$(call mktest,$@,$(SAMDOBJS),$(TEST_LIBS))

clean::
	rm -f TestI.cpp TestI.h
	rm -f TestAMDI.cpp TestAMDI.h
