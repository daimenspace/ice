# **********************************************************************
#
# Copyright (c) 2003-2016 ZeroC, Inc. All rights reserved.
#
# This copy of Ice is licensed to you under the terms described in the
# ICE_LICENSE file included in this distribution.
#
# **********************************************************************

top_srcdir	= ../..

include $(top_srcdir)/config/Make.rules

SUBDIRS		= proxy \
                  operations \
                  exceptions \
                  ami \
                  info \
                  inheritance \
                  facets \
                  objects \
                  faultTolerance \
                  location \
                  adapterDeactivation \
                  slicing \
                  hash \
                  checksum \
                  dispatcher \
                  hold \
                  binding \
                  retry \
                  timeout \
                  acm \
                  servantLocator \
                  interceptor \
                  stringConverter \
                  background \
                  udp \
                  defaultServant \
                  defaultValue \
                  threadPoolPriority \
                  invoke \
                  properties \
                  plugin \
                  admin \
                  metrics \
                  enums \
                  logger \
                  networkProxy \
                  impl \
                  stream

ifneq ($(CPP11_MAPPING),yes)
SUBDIRS		:= $(SUBDIRS) \
		  optional \
		  gc \
		  custom \
		  echo
endif

ifeq ($(findstring MINGW,$(UNAME)),)
SUBDIRS := $(SUBDIRS) services
endif

.PHONY: $(EVERYTHING) $(SUBDIRS)

all:: $(SUBDIRS)

$(SUBDIRS):
	@echo "making all in $@"
	@$(MAKE) all --directory=$@

$(EVERYTHING_EXCEPT_ALL)::
	@for subdir in $(SUBDIRS); \
	do \
	    echo "making $@ in $$subdir"; \
	    ( cd $$subdir && $(MAKE) $@ ) || exit 1; \
	done
