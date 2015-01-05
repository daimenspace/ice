// **********************************************************************
//
// Copyright (c) 2003-2015 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

package IceSSL;

class PluginI implements Plugin
{
    public PluginI(Ice.Communicator communicator)
    {
        IceInternal.ProtocolPluginFacade facade = IceInternal.Util.getProtocolPluginFacade(communicator);

        _engine = new SSLEngine(facade);

        //
        // Register the endpoint factory. We have to do this now, rather than
        // in initialize, because the communicator may need to interpret
        // proxies before the plug-in is fully initialized.
        //
        IceInternal.EndpointFactory sslEndpointFactory =
                                    new EndpointFactoryI(new Instance(_engine, IceSSL.EndpointType.value, "ssl"));
        facade.addEndpointFactory(sslEndpointFactory);
            
            
        IceInternal.ProtocolInstance wssProtocolInstance = 
                                    new IceInternal.ProtocolInstance(communicator, Ice.WSSEndpointType.value, "wss");
        IceInternal.EndpointFactory wssEndpointFactory = 
                                    new IceInternal.WSEndpointFactory(wssProtocolInstance, 
                                                                      sslEndpointFactory.clone(wssProtocolInstance));
        facade.addEndpointFactory(wssEndpointFactory);
    }

    @Override
    public void initialize()
    {
        _engine.initialize();
    }

    @Override
    public void destroy()
    {
    }

    @Override
    public void setContext(javax.net.ssl.SSLContext context)
    {
        _engine.context(context);
    }

    @Override
    public javax.net.ssl.SSLContext getContext()
    {
        return _engine.context();
    }

    @Override
    public void setCertificateVerifier(CertificateVerifier verifier)
    {
        _engine.setCertificateVerifier(verifier);
    }

    @Override
    public CertificateVerifier getCertificateVerifier()
    {
        return _engine.getCertificateVerifier();
    }

    @Override
    public void setPasswordCallback(PasswordCallback callback)
    {
        _engine.setPasswordCallback(callback);
    }

    @Override
    public PasswordCallback getPasswordCallback()
    {
        return _engine.getPasswordCallback();
    }

    @Override
    public void setKeystoreStream(java.io.InputStream stream)
    {
        _engine.setKeystoreStream(stream);
    }

    @Override
    public void setTruststoreStream(java.io.InputStream stream)
    {
        _engine.setTruststoreStream(stream);
    }

    @Override
    public void addSeedStream(java.io.InputStream stream)
    {
        _engine.addSeedStream(stream);
    }

    private SSLEngine _engine;
}
