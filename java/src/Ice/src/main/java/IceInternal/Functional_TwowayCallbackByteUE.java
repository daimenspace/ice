// **********************************************************************
//
// Copyright (c) 2003-2015 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

package IceInternal;

public abstract class Functional_TwowayCallbackByteUE
    extends Functional_TwowayCallbackByte implements Ice.TwowayCallbackByteUE
{
    public Functional_TwowayCallbackByteUE(
        Functional_ByteCallback responseCb,
        Functional_GenericCallback1<Ice.UserException> userExceptionCb,
        Functional_GenericCallback1<Ice.Exception> exceptionCb,
        Functional_BoolCallback sentCb)
    {
        super(userExceptionCb != null, responseCb, exceptionCb, sentCb);
        __userExceptionCb = userExceptionCb;
    }

    @Override
    public void exception(Ice.UserException ex)
    {
        if(__userExceptionCb != null)
        {
            __userExceptionCb.apply(ex);
        }
    }

    private final Functional_GenericCallback1<Ice.UserException> __userExceptionCb;
}
