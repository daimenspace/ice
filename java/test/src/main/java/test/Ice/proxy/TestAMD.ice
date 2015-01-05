// **********************************************************************
//
// Copyright (c) 2003-2015 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

#pragma once

#include<Ice/Current.ice>

[["java:package:test.Ice.proxy.AMD"]]
module Test
{

["amd"] class MyClass
{
    void shutdown();

    Ice::Context getContext();
};

["amd"] class MyDerivedClass extends MyClass
{
    Object* echo(Object* obj);
};

};
