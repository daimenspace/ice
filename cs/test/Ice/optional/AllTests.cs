// **********************************************************************
//
// Copyright (c) 2003-2012 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

using System;
using System.Collections.Generic;

#if SILVERLIGHT
using System.Windows.Controls;
#endif

public class AllTests : TestCommon.TestApp
{
#if SILVERLIGHT
    override
    public void run(Ice.Communicator communicator)
#else
    public static Test.InitialPrx allTests(Ice.Communicator communicator)
#endif
    {
        FactoryI factory = new FactoryI();
        communicator.addObjectFactory(factory, "");

        Write("testing stringToProxy... ");
        Flush();
        string @ref = "initial:default -p 12010";
        Ice.ObjectPrx @base = communicator.stringToProxy(@ref);
        test(@base != null);
        WriteLine("ok");

        Write("testing checked cast... ");
        Flush();
        Test.InitialPrx initial = Test.InitialPrxHelper.checkedCast(@base);
        test(initial != null);
        test(initial.Equals(@base));
        WriteLine("ok");

        Write("testing optional data members... ");
        Flush();

        Test.OneOptional oo1 = new Test.OneOptional();
        test(!oo1.hasA);
        oo1.a = 15;
        test(oo1.hasA && oo1.a == 15);

        Test.OneOptional oo2 = new Test.OneOptional(16);
        test(oo2.hasA && oo2.a == 16);

        Test.MultiOptional mo1 = new Test.MultiOptional();
        mo1.a = 15;
        mo1.b = true;
        mo1.c = 19;
        mo1.d = 78;
        mo1.e = 99;
        mo1.f = (float)5.5;
        mo1.g = 1.0;
        mo1.h = "test";
        mo1.i = Test.MyEnum.MyEnumMember;
        mo1.j = Test.MultiOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test"));
        mo1.k = mo1;
        mo1.bs = new byte[] { 5 };
        mo1.ss = new string[] { "test", "test2" };
        mo1.iid = new Dictionary<int, int>();
        mo1.iid.Add(4, 3);
        mo1.sid = new Dictionary<string, int>();
        mo1.sid.Add("test", 10);
        Test.FixedStruct fs = new Test.FixedStruct();
        fs.m = 78;
        mo1.fs = fs;
        Test.VarStruct vs = new Test.VarStruct();
        vs.m = "hello";
        mo1.vs = vs;

        mo1.shs = new short[] { 1 };
        mo1.es = new Test.MyEnum[] { Test.MyEnum.MyEnumMember, Test.MyEnum.MyEnumMember };
        mo1.fss = new Test.FixedStruct[] { fs };
        mo1.vss = new Test.VarStruct[] { vs };
        mo1.oos = new Test.OneOptional[] { oo1 };
        mo1.oops = new Test.OneOptionalPrx[]
            { Test.OneOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test")) };

        mo1.ied = new Dictionary<int, Test.MyEnum>();
        mo1.ied.Add(4, Test.MyEnum.MyEnumMember);
        mo1.ifsd = new Dictionary<int, Test.FixedStruct>();
        mo1.ifsd.Add(4, fs);
        mo1.ivsd = new Dictionary<int, Test.VarStruct>();
        mo1.ivsd.Add(5, vs);
        mo1.iood = new Dictionary<int, Test.OneOptional>();
        mo1.iood.Add(5, new Test.OneOptional(15));
        mo1.ioopd = new Dictionary<int, Test.OneOptionalPrx>();
        mo1.ioopd.Add(5, Test.OneOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test")));

        mo1.bos = new bool[] { false, true, false };

#if !SILVERLIGHT
        mo1.ser = new Test.SerializableClass(56);
#endif

        test(mo1.a == (byte)15);
        test(mo1.b);
        test(mo1.c == 19);
        test(mo1.d == 78);
        test(mo1.e == 99);
        test(mo1.f == (float)5.5);
        test(mo1.g == 1.0);
        test(mo1.h.Equals("test"));
        test(mo1.i == Test.MyEnum.MyEnumMember);
        test(mo1.j.Equals(Test.MultiOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test"))));
        test(mo1.k == mo1);
        test(ArraysEqual(mo1.bs, new byte[] { (byte)5 }));
        test(ArraysEqual(mo1.ss, new String[] { "test", "test2" }));
        test(mo1.iid[4] == 3);
        test(mo1.sid["test"] == 10);
        test(mo1.fs.Equals(new Test.FixedStruct(78)));
        test(mo1.vs.Equals(new Test.VarStruct("hello")));

        test(mo1.shs[0] == (short)1);
        test(mo1.es[0] == Test.MyEnum.MyEnumMember && mo1.es[1] == Test.MyEnum.MyEnumMember);
        test(mo1.fss[0].Equals(new Test.FixedStruct(78)));
        test(mo1.vss[0].Equals(new Test.VarStruct("hello")));
        test(mo1.oos[0] == oo1);
        test(mo1.oops[0].Equals(Test.OneOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test"))));

        test(mo1.ied[4] == Test.MyEnum.MyEnumMember);
        test(mo1.ifsd[4].Equals(new Test.FixedStruct(78)));
        test(mo1.ivsd[5].Equals(new Test.VarStruct("hello")));
        test(mo1.iood[5].a == 15);
        test(mo1.ioopd[5].Equals(Test.OneOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test"))));

        test(ArraysEqual(mo1.bos, new bool[] { false, true, false }));

#if !SILVERLIGHT
        test(mo1.ser.Equals(new Test.SerializableClass(56)));
#endif

        WriteLine("ok");

        Write("testing marshaling... ");
        Flush();

        Test.OneOptional oo4 = (Test.OneOptional)initial.pingPong(new Test.OneOptional());
        test(!oo4.hasA);

        Test.OneOptional oo5 = (Test.OneOptional)initial.pingPong(oo1);
        test(oo1.a == oo5.a);

        Test.MultiOptional mo4 = (Test.MultiOptional)initial.pingPong(new Test.MultiOptional());
        test(!mo4.hasA);
        test(!mo4.hasB);
        test(!mo4.hasC);
        test(!mo4.hasD);
        test(!mo4.hasE);
        test(!mo4.hasF);
        test(!mo4.hasG);
        test(!mo4.hasH);
        test(!mo4.hasI);
        test(!mo4.hasJ);
        test(!mo4.hasK);
        test(!mo4.hasBs);
        test(!mo4.hasSs);
        test(!mo4.hasIid);
        test(!mo4.hasSid);
        test(!mo4.hasFs);
        test(!mo4.hasVs);

        test(!mo4.hasShs);
        test(!mo4.hasEs);
        test(!mo4.hasFss);
        test(!mo4.hasVss);
        test(!mo4.hasOos);
        test(!mo4.hasOops);

        test(!mo4.hasIed);
        test(!mo4.hasIfsd);
        test(!mo4.hasIvsd);
        test(!mo4.hasIood);
        test(!mo4.hasIoopd);

        test(!mo4.hasBos);

#if !SILVERLIGHT
        test(!mo4.hasSer);
#endif

        Test.MultiOptional mo5 = (Test.MultiOptional)initial.pingPong(mo1);
        test(mo5.a == mo1.a);
        test(mo5.b == mo1.b);
        test(mo5.c == mo1.c);
        test(mo5.d == mo1.d);
        test(mo5.e == mo1.e);
        test(mo5.f == mo1.f);
        test(mo5.g == mo1.g);
        test(mo5.h.Equals(mo1.h));
        test(mo5.i == mo1.i);
        test(mo5.j.Equals(mo1.j));
        test(mo5.k == mo5);
        test(ArraysEqual(mo5.bs, mo1.bs));
        test(ArraysEqual(mo5.ss, mo1.ss));
        test(mo5.iid[4] == 3);
        test(mo5.sid["test"] == 10);
        test(mo5.fs.Equals(mo1.fs));
        test(mo5.vs.Equals(mo1.vs));
        test(ArraysEqual(mo5.shs, mo1.shs));
        test(mo5.es[0] == Test.MyEnum.MyEnumMember && mo1.es[1] == Test.MyEnum.MyEnumMember);
        test(mo5.fss[0].Equals(new Test.FixedStruct(78)));
        test(mo5.vss[0].Equals(new Test.VarStruct("hello")));
        test(mo5.oos[0].a == 15);
        test(mo5.oops[0].Equals(Test.OneOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test"))));

        test(mo5.ied[4] == Test.MyEnum.MyEnumMember);
        test(mo5.ifsd[4].Equals(new Test.FixedStruct(78)));
        test(mo5.ivsd[5].Equals(new Test.VarStruct("hello")));
        test(mo5.iood[5].a == 15);
        test(mo5.ioopd[5].Equals(Test.OneOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test"))));

        test(ArraysEqual(mo5.bos, new bool[] { false, true, false }));

#if !SILVERLIGHT
        test(mo5.ser.Equals(new Test.SerializableClass(56)));
#endif

        // Clear the first half of the optional members
        Test.MultiOptional mo6 = new Test.MultiOptional();
        mo6.b = mo5.b;
        mo6.d = mo5.d;
        mo6.f = mo5.f;
        mo6.h = mo5.h;
        mo6.j = mo5.j;
        mo6.bs = mo5.bs;
        mo6.iid = mo5.iid;
        mo6.fs = mo5.fs;
        mo6.shs = mo5.shs;
        mo6.fss = mo5.fss;
        mo6.oos = mo5.oos;
        mo6.ifsd = mo5.ifsd;
        mo6.iood = mo5.iood;
        mo6.bos = mo5.bos;

        Test.MultiOptional mo7 = (Test.MultiOptional)initial.pingPong(mo6);
        test(!mo7.hasA);
        test(mo7.b == mo1.b);
        test(!mo7.hasC);
        test(mo7.d == mo1.d);
        test(!mo7.hasE);
        test(mo7.f == mo1.f);
        test(!mo7.hasG);
        test(mo7.h.Equals(mo1.h));
        test(!mo7.hasI);
        test(mo7.j.Equals(mo1.j));
        test(!mo7.hasK);
        test(ArraysEqual(mo7.bs, mo1.bs));
        test(!mo7.hasSs);
        test(mo7.iid[4] == 3);
        test(!mo7.hasSid);
        test(mo7.fs.Equals(mo1.fs));
        test(!mo7.hasVs);

        test(ArraysEqual(mo7.shs, mo1.shs));
        test(!mo7.hasEs);
        test(mo7.fss[0].Equals(new Test.FixedStruct(78)));
        test(!mo7.hasVss);
        test(mo7.oos[0].a == 15);
        test(!mo7.hasOops);

        test(!mo7.hasIed);
        test(mo7.ifsd[4].Equals(new Test.FixedStruct(78)));
        test(!mo7.hasIvsd);
        test(mo7.iood[5].a == 15);
        test(!mo7.hasIoopd);

        test(ArraysEqual(mo7.bos, new bool[] { false, true, false }));

#if !SILVERLIGHT
        test(!mo7.hasSer);
#endif

        // Clear the second half of the optional members
        Test.MultiOptional mo8 = new Test.MultiOptional();
        mo8.a = mo5.a;
        mo8.c = mo5.c;
        mo8.e = mo5.e;
        mo8.g = mo5.g;
        mo8.i = mo5.i;
        mo8.k = mo8;
        mo8.ss = mo5.ss;
        mo8.sid = mo5.sid;
        mo8.vs = mo5.vs;

        mo8.es = mo5.es;
        mo8.vss = mo5.vss;
        mo8.oops = mo5.oops;

        mo8.ied = mo5.ied;
        mo8.ivsd = mo5.ivsd;
        mo8.ioopd = mo5.ioopd;

#if !SILVERLIGHT
        mo8.ser = new Test.SerializableClass(56);
#endif

        Test.MultiOptional mo9 = (Test.MultiOptional)initial.pingPong(mo8);
        test(mo9.a == mo1.a);
        test(!mo9.hasB);
        test(mo9.c == mo1.c);
        test(!mo9.hasD);
        test(mo9.e == mo1.e);
        test(!mo9.hasF);
        test(mo9.g == mo1.g);
        test(!mo9.hasH);
        test(mo9.i == mo1.i);
        test(!mo9.hasJ);
        test(mo9.k == mo9);
        test(!mo9.hasBs);
        test(ArraysEqual(mo9.ss, mo1.ss));
        test(!mo9.hasIid);
        test(mo9.sid["test"] == 10);
        test(!mo9.hasFs);
        test(mo9.vs.Equals(mo1.vs));

        test(!mo9.hasShs);
        test(mo9.es[0] == Test.MyEnum.MyEnumMember && mo1.es[1] == Test.MyEnum.MyEnumMember);
        test(!mo9.hasFss);
        test(mo9.vss[0].Equals(new Test.VarStruct("hello")));
        test(!mo9.hasOos);
        test(mo9.oops[0].Equals(Test.OneOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test"))));

        test(mo9.ied[4] == Test.MyEnum.MyEnumMember);
        test(!mo9.hasIfsd);
        test(mo9.ivsd[5].Equals(new Test.VarStruct("hello")));
        test(!mo9.hasIood);
        test(mo9.ioopd[5].Equals(Test.OneOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test"))));

        test(!mo9.hasBos);

#if !SILVERLIGHT
        test(mo9.ser.Equals(new Test.SerializableClass(56)));
#endif

        {
            Test.OptionalWithCustom owc1 = new Test.OptionalWithCustom();
            owc1.l = new List<Test.SmallStruct>();
            owc1.l.Add(new Test.SmallStruct(5));
            owc1.l.Add(new Test.SmallStruct(6));
            owc1.l.Add(new Test.SmallStruct(7));
            owc1.s = new Test.ClassVarStruct(5);
            Test.OptionalWithCustom owc2 = (Test.OptionalWithCustom)initial.pingPong(owc1);
            test(owc2.hasL);
            test(ListsEqual(owc1.l, owc2.l));
            test(owc2.hasS);
            test(owc2.s.a == 5);
        }

        //
        // Send a request using blobjects. Upon receival, we don't read
        // any of the optional members. This ensures the optional members
        // are skipped even if the receiver knows nothing about them.
        //
        factory.setEnabled(true);
        Ice.OutputStream os = Ice.Util.createOutputStream(communicator);
        os.startEncapsulation();
        os.writeObject(oo1);
        os.endEncapsulation();
        byte[] inEncaps = os.finished();
        byte[] outEncaps;
        test(initial.ice_invoke("pingPong", Ice.OperationMode.Normal, inEncaps, out outEncaps));
        Ice.InputStream @in = Ice.Util.createInputStream(communicator, outEncaps);
        @in.startEncapsulation();
        ReadObjectCallbackI cb = new ReadObjectCallbackI();
        @in.readObject(cb);
        @in.endEncapsulation();
        test(cb.obj != null && cb.obj is TestObjectReader);

        os = Ice.Util.createOutputStream(communicator);
        os.startEncapsulation();
        os.writeObject(mo1);
        os.endEncapsulation();
        inEncaps = os.finished();
        test(initial.ice_invoke("pingPong", Ice.OperationMode.Normal, inEncaps, out outEncaps));
        @in = Ice.Util.createInputStream(communicator, outEncaps);
        @in.startEncapsulation();
        @in.readObject(cb);
        @in.endEncapsulation();
        test(cb.obj != null && cb.obj is TestObjectReader);
        factory.setEnabled(false);

        WriteLine("ok");

        Write("testing marshaling of large containers with fixed size elements... ");
        Flush();
        Test.MultiOptional mc = new Test.MultiOptional();

        mc.bs = new byte[1000];
        mc.shs = new short[300];

        mc.fss = new Test.FixedStruct[300];
        for(int i = 0; i < 300; ++i)
        {
            mc.fss[i] = new Test.FixedStruct();
        }

        mc.ifsd = new Dictionary<int, Test.FixedStruct>();
        for(int i = 0; i < 300; ++i)
        {
            mc.ifsd.Add(i, new Test.FixedStruct());
        }

        mc = (Test.MultiOptional)initial.pingPong(mc);
        test(mc.bs.Length == 1000);
        test(mc.shs.Length == 300);
        test(mc.fss.Length == 300);
        test(mc.ifsd.Count == 300);

        factory.setEnabled(true);
        os = Ice.Util.createOutputStream(communicator);
        os.startEncapsulation();
        os.writeObject(mc);
        os.endEncapsulation();
        inEncaps = os.finished();
        test(initial.ice_invoke("pingPong", Ice.OperationMode.Normal, inEncaps, out outEncaps));
        @in = Ice.Util.createInputStream(communicator, outEncaps);
        @in.startEncapsulation();
        @in.readObject(cb);
        @in.endEncapsulation();
        test(cb.obj != null && cb.obj is TestObjectReader);
        factory.setEnabled(false);

        WriteLine("ok");

        Write("testing tag marshaling... ");
        Flush();
        {
            Test.B b = new Test.B();
            Test.B b2 = (Test.B)initial.pingPong(b);
            test(!b2.hasMa);
            test(!b2.hasMb);
            test(!b2.hasMc);

            b.ma = 10;
            b.mb = 11;
            b.mc = 12;
            b.md = 13;

            b2 = (Test.B)initial.pingPong(b);
            test(b2.ma == 10);
            test(b2.mb == 11);
            test(b2.mc == 12);
            test(b2.md == 13);

            factory.setEnabled(true);
            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeObject(b);
            os.endEncapsulation();
            inEncaps = os.finished();
            test(initial.ice_invoke("pingPong", Ice.OperationMode.Normal, inEncaps, out outEncaps));
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.readObject(cb);
            @in.endEncapsulation();
            test(cb.obj != null);
            factory.setEnabled(false);
        }
        WriteLine("ok");

        Write("testing optional with default values... ");
        Flush();
        {
            Test.WD wd = (Test.WD)initial.pingPong(new Test.WD());
            test(wd.a == 5);
            test(wd.s.Equals("test"));
            wd.clearA();
            wd.clearS();
            wd = (Test.WD)initial.pingPong(wd);
            test(!wd.hasA);
            test(!wd.hasS);
        }
        WriteLine("ok");

        if(communicator.getProperties().getPropertyAsInt("Ice.Default.SlicedFormat") > 0)
        {
            Write("testing marshaling with unknown class slices... ");
            Flush();
            {
                Test.C c = new Test.C();
                c.ss = "test";
                c.ms = "testms";
                os = Ice.Util.createOutputStream(communicator);
                os.startEncapsulation();
                os.writeObject(c);
                os.endEncapsulation();
                inEncaps = os.finished();
                factory.setEnabled(true);
                test(initial.ice_invoke("pingPong", Ice.OperationMode.Normal, inEncaps, out outEncaps));
                @in = Ice.Util.createInputStream(communicator, outEncaps);
                @in.startEncapsulation();
                @in.readObject(cb);
                @in.endEncapsulation();
                test(cb.obj is CObjectReader);
                factory.setEnabled(false);

                factory.setEnabled(true);
                os = Ice.Util.createOutputStream(communicator);
                os.startEncapsulation();
                Ice.Object d = new DObjectWriter();
                os.writeObject(d);
                os.endEncapsulation();
                inEncaps = os.finished();
                test(initial.ice_invoke("pingPong", Ice.OperationMode.Normal, inEncaps, out outEncaps));
                @in = Ice.Util.createInputStream(communicator, outEncaps);
                @in.startEncapsulation();
                @in.readObject(cb);
                @in.endEncapsulation();
                test(cb.obj != null && cb.obj is DObjectReader);
                ((DObjectReader)cb.obj).check();
                factory.setEnabled(false);
            }
            WriteLine("ok");

            Write("testing optionals with unknown classes...");
            Flush();
            {
                Test.A a = new Test.A();

                os = Ice.Util.createOutputStream(communicator);
                os.startEncapsulation();
                os.writeObject(a);
                os.writeOptional(1, Ice.OptionalType.Size);
                os.writeObject(new DObjectWriter());
                os.endEncapsulation();
                inEncaps = os.finished();
                test(initial.ice_invoke("opClassAndUnknownOptional", Ice.OperationMode.Normal, inEncaps,
                                        out outEncaps));

                @in = Ice.Util.createInputStream(communicator, outEncaps);
                @in.startEncapsulation();
                @in.endEncapsulation();
            }
            WriteLine("ok");
        }

        Write("testing optional parameters... ");
        Flush();
        {
            Ice.Optional<byte> p1 = new Ice.Optional<byte>();
            Ice.Optional<byte> p3;
            Ice.Optional<byte> p2 = initial.opByte(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opByte(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = 56;
            p2 = initial.opByte(p1, out p3);
            test(p2.Value == 56 && p3.Value == 56);
            Ice.AsyncResult r = initial.begin_opByte(p1);
            p2 = initial.end_opByte(out p3, r);
            test(p2.Value == 56 && p3.Value == 56);
            p2 = initial.opByte(p1.Value, out p3);
            test(p2.Value == 56 && p3.Value == 56);
            r = initial.begin_opByte(p1.Value);
            p2 = initial.end_opByte(out p3, r);
            test(p2.Value == 56 && p3.Value == 56);

            p2 = initial.opByte(new Ice.Optional<byte>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.F1);
            os.writeByte(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opByte", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.F1));
            test(@in.readByte() == 56);
            test(@in.readOptional(3, Ice.OptionalType.F1));
            test(@in.readByte() == 56);
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<bool> p1 = new Ice.Optional<bool>();
            Ice.Optional<bool> p3;
            Ice.Optional<bool> p2 = initial.opBool(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opBool(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = true;
            p2 = initial.opBool(p1, out p3);
            test(p2.Value == true && p3.Value == true);
            Ice.AsyncResult r = initial.begin_opBool(p1);
            p2 = initial.end_opBool(out p3, r);
            test(p2.Value == true && p3.Value == true);
            p2 = initial.opBool(true, out p3);
            test(p2.Value == true && p3.Value == true);
            r = initial.begin_opBool(true);
            p2 = initial.end_opBool(out p3, r);
            test(p2.Value == true && p3.Value == true);

            p2 = initial.opBool(new Ice.Optional<bool>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.F1);
            os.writeBool(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opBool", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.F1));
            test(@in.readBool() == true);
            test(@in.readOptional(3, Ice.OptionalType.F1));
            test(@in.readBool() == true);
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<short> p1 = new Ice.Optional<short>();
            Ice.Optional<short> p3;
            Ice.Optional<short> p2 = initial.opShort(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opShort(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = 56;
            p2 = initial.opShort(p1, out p3);
            test(p2.Value == 56 && p3.Value == 56);
            Ice.AsyncResult r = initial.begin_opShort(p1);
            p2 = initial.end_opShort(out p3, r);
            test(p2.Value == 56 && p3.Value == 56);
            p2 = initial.opShort(p1.Value, out p3);
            test(p2.Value == 56 && p3.Value == 56);
            r = initial.begin_opShort(p1.Value);
            p2 = initial.end_opShort(out p3, r);
            test(p2.Value == 56 && p3.Value == 56);

            p2 = initial.opShort(new Ice.Optional<short>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.F2);
            os.writeShort(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opShort", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.F2));
            test(@in.readShort() == 56);
            test(@in.readOptional(3, Ice.OptionalType.F2));
            test(@in.readShort() == 56);
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<int> p1 = new Ice.Optional<int>();
            Ice.Optional<int> p3;
            Ice.Optional<int> p2 = initial.opInt(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opInt(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = 56;
            p2 = initial.opInt(p1, out p3);
            test(p2.Value == 56 && p3.Value == 56);
            Ice.AsyncResult r = initial.begin_opInt(p1);
            p2 = initial.end_opInt(out p3, r);
            test(p2.Value == 56 && p3.Value == 56);
            p2 = initial.opInt(p1.Value, out p3);
            test(p2.Value == 56 && p3.Value == 56);
            r = initial.begin_opInt(p1.Value);
            p2 = initial.end_opInt(out p3, r);
            test(p2.Value == 56 && p3.Value == 56);

            p2 = initial.opInt(new Ice.Optional<int>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.F4);
            os.writeInt(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opInt", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.F4));
            test(@in.readInt() == 56);
            test(@in.readOptional(3, Ice.OptionalType.F4));
            test(@in.readInt() == 56);
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<long> p1 = new Ice.Optional<long>();
            Ice.Optional<long> p3;
            Ice.Optional<long> p2 = initial.opLong(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opLong(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = 56;
            p2 = initial.opLong(p1, out p3);
            test(p2.Value == 56 && p3.Value == 56);
            Ice.AsyncResult r = initial.begin_opLong(p1);
            p2 = initial.end_opLong(out p3, r);
            test(p2.Value == 56 && p3.Value == 56);
            p2 = initial.opLong(p1.Value, out p3);
            test(p2.Value == 56 && p3.Value == 56);
            r = initial.begin_opLong(p1.Value);
            p2 = initial.end_opLong(out p3, r);
            test(p2.Value == 56 && p3.Value == 56);

            p2 = initial.opLong(new Ice.Optional<long>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.F8);
            os.writeLong(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opLong", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.F8));
            test(@in.readLong() == 56);
            test(@in.readOptional(3, Ice.OptionalType.F8));
            test(@in.readLong() == 56);
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<float> p1 = new Ice.Optional<float>();
            Ice.Optional<float> p3;
            Ice.Optional<float> p2 = initial.opFloat(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opFloat(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = (float)1.0;
            p2 = initial.opFloat(p1, out p3);
            test(p2.Value == 1.0 && p3.Value == 1.0);
            Ice.AsyncResult r = initial.begin_opFloat(p1);
            p2 = initial.end_opFloat(out p3, r);
            test(p2.Value == 1.0 && p3.Value == 1.0);
            p2 = initial.opFloat(p1.Value, out p3);
            test(p2.Value == 1.0 && p3.Value == 1.0);
            r = initial.begin_opFloat(p1.Value);
            p2 = initial.end_opFloat(out p3, r);
            test(p2.Value == 1.0 && p3.Value == 1.0);

            p2 = initial.opFloat(new Ice.Optional<float>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.F4);
            os.writeFloat(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opFloat", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.F4));
            test(@in.readFloat() == 1.0);
            test(@in.readOptional(3, Ice.OptionalType.F4));
            test(@in.readFloat() == 1.0);
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<double> p1 = new Ice.Optional<double>();
            Ice.Optional<double> p3;
            Ice.Optional<double> p2 = initial.opDouble(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opDouble(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = 1.0;
            p2 = initial.opDouble(p1, out p3);
            test(p2.Value == 1.0 && p3.Value == 1.0);
            Ice.AsyncResult r = initial.begin_opDouble(p1);
            p2 = initial.end_opDouble(out p3, r);
            test(p2.Value == 1.0 && p3.Value == 1.0);
            p2 = initial.opDouble(p1.Value, out p3);
            test(p2.Value == 1.0 && p3.Value == 1.0);
            r = initial.begin_opDouble(p1.Value);
            p2 = initial.end_opDouble(out p3, r);
            test(p2.Value == 1.0 && p3.Value == 1.0);

            p2 = initial.opDouble(new Ice.Optional<double>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.F8);
            os.writeDouble(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opDouble", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.F8));
            test(@in.readDouble() == 1.0);
            test(@in.readOptional(3, Ice.OptionalType.F8));
            test(@in.readDouble() == 1.0);
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<string> p1 = new Ice.Optional<string>();
            Ice.Optional<string> p3;
            Ice.Optional<string> p2 = initial.opString(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opString(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opString(null, out p3); // Implicitly converts to Ice.Optional<string>(null)
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = "test";
            p2 = initial.opString(p1, out p3);
            test(p2.Value.Equals("test") && p3.Value.Equals("test"));
            Ice.AsyncResult r = initial.begin_opString(p1);
            p2 = initial.end_opString(out p3, r);
            test(p2.Value.Equals("test") && p3.Value.Equals("test"));
            p2 = initial.opString(p1.Value, out p3);
            test(p2.Value.Equals("test") && p3.Value.Equals("test"));
            r = initial.begin_opString(p1.Value);
            p2 = initial.end_opString(out p3, r);
            test(p2.Value.Equals("test") && p3.Value.Equals("test"));

            p2 = initial.opString(new Ice.Optional<string>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeString(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opString", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            test(@in.readString().Equals("test"));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            test(@in.readString().Equals("test"));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<Test.MyEnum> p1 = new Ice.Optional<Test.MyEnum>();
            Ice.Optional<Test.MyEnum> p3;
            Ice.Optional<Test.MyEnum> p2 = initial.opMyEnum(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opMyEnum(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = Test.MyEnum.MyEnumMember;
            p2 = initial.opMyEnum(p1, out p3);
            test(p2.Value == Test.MyEnum.MyEnumMember && p3.Value == Test.MyEnum.MyEnumMember);
            Ice.AsyncResult r = initial.begin_opMyEnum(p1);
            p2 = initial.end_opMyEnum(out p3, r);
            test(p2.Value == Test.MyEnum.MyEnumMember && p3.Value == Test.MyEnum.MyEnumMember);
            p2 = initial.opMyEnum(p1.Value, out p3);
            test(p2.Value == Test.MyEnum.MyEnumMember && p3.Value == Test.MyEnum.MyEnumMember);
            r = initial.begin_opMyEnum(p1.Value);
            p2 = initial.end_opMyEnum(out p3, r);
            test(p2.Value == Test.MyEnum.MyEnumMember && p3.Value == Test.MyEnum.MyEnumMember);

            p2 = initial.opMyEnum(new Ice.Optional<Test.MyEnum>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.Size);
            os.writeEnum((int)p1.Value, 1);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opMyEnum", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.Size));
            test((Test.MyEnum)@in.readEnum(1) == Test.MyEnum.MyEnumMember);
            test(@in.readOptional(3, Ice.OptionalType.Size));
            test((Test.MyEnum)@in.readEnum(1) == Test.MyEnum.MyEnumMember);
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<Test.SmallStruct> p1 = new Ice.Optional<Test.SmallStruct>();
            Ice.Optional<Test.SmallStruct> p3;
            Ice.Optional<Test.SmallStruct> p2 = initial.opSmallStruct(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opSmallStruct(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = new Test.SmallStruct(56);
            p2 = initial.opSmallStruct(p1, out p3);
            test(p2.Value.m == (byte)56 && p3.Value.m == (byte)56);
            Ice.AsyncResult r = initial.begin_opSmallStruct(p1);
            p2 = initial.end_opSmallStruct(out p3, r);
            test(p2.Value.m == (byte)56 && p3.Value.m == (byte)56);
            p2 = initial.opSmallStruct(p1.Value, out p3);
            test(p2.Value.m == (byte)56 && p3.Value.m == (byte)56);
            r = initial.begin_opSmallStruct(p1.Value);
            p2 = initial.end_opSmallStruct(out p3, r);
            test(p2.Value.m == (byte)56 && p3.Value.m == (byte)56);

            p2 = initial.opSmallStruct(new Ice.Optional<Test.SmallStruct>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(1);
            p1.Value.ice_write(os);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opSmallStruct", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            Test.SmallStruct f = new Test.SmallStruct();
            f.ice_read(@in);
            test(f.m == (byte)56);
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            f.ice_read(@in);
            test(f.m == (byte)56);
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<Test.FixedStruct> p1 = new Ice.Optional<Test.FixedStruct>();
            Ice.Optional<Test.FixedStruct> p3;
            Ice.Optional<Test.FixedStruct> p2 = initial.opFixedStruct(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opFixedStruct(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = new Test.FixedStruct(56);
            p2 = initial.opFixedStruct(p1, out p3);
            test(p2.Value.m == 56 && p3.Value.m == 56);
            Ice.AsyncResult r = initial.begin_opFixedStruct(p1);
            p2 = initial.end_opFixedStruct(out p3, r);
            test(p2.Value.m == 56 && p3.Value.m == 56);
            p2 = initial.opFixedStruct(p1.Value, out p3);
            test(p2.Value.m == 56 && p3.Value.m == 56);
            r = initial.begin_opFixedStruct(p1.Value);
            p2 = initial.end_opFixedStruct(out p3, r);
            test(p2.Value.m == 56 && p3.Value.m == 56);

            p2 = initial.opFixedStruct(new Ice.Optional<Test.FixedStruct>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(4);
            p1.Value.ice_write(os);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opFixedStruct", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            Test.FixedStruct f = new Test.FixedStruct();
            f.ice_read(@in);
            test(f.m == 56);
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            f.ice_read(@in);
            test(f.m == 56);
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<Test.VarStruct> p1 = new Ice.Optional<Test.VarStruct>();
            Ice.Optional<Test.VarStruct> p3;
            Ice.Optional<Test.VarStruct> p2 = initial.opVarStruct(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opVarStruct(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = new Test.VarStruct("test");
            p2 = initial.opVarStruct(p1, out p3);
            test(p2.Value.m.Equals("test") && p3.Value.m.Equals("test"));
            Ice.AsyncResult r = initial.begin_opVarStruct(p1);
            p2 = initial.end_opVarStruct(out p3, r);
            test(p2.Value.m.Equals("test") && p3.Value.m.Equals("test"));
            p2 = initial.opVarStruct(p1.Value, out p3);
            test(p2.Value.m.Equals("test") && p3.Value.m.Equals("test"));
            r = initial.begin_opVarStruct(p1.Value);
            p2 = initial.end_opVarStruct(out p3, r);
            test(p2.Value.m.Equals("test") && p3.Value.m.Equals("test"));

            p2 = initial.opVarStruct(new Ice.Optional<Test.VarStruct>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.FSize);
            os.startSize();
            p1.Value.ice_write(os);
            os.endSize();
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opVarStruct", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.FSize));
            @in.skip(4);
            Test.VarStruct v = new Test.VarStruct();
            v.ice_read(@in);
            test(v.m.Equals("test"));
            test(@in.readOptional(3, Ice.OptionalType.FSize));
            @in.skip(4);
            v.ice_read(@in);
            test(v.m.Equals("test"));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<Test.OneOptional> p1 = new Ice.Optional<Test.OneOptional>();
            Ice.Optional<Test.OneOptional> p3;
            Ice.Optional<Test.OneOptional> p2 = initial.opOneOptional(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opOneOptional(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opOneOptional(null, out p3); // Implicitly converts to Ice.Optional<OneOptional>(null)
            test(p2.HasValue && p2.Value == null && p3.HasValue && p3.Value == null);

            p2 = initial.opOneOptional(new Ice.Optional<Test.OneOptional>((Test.OneOptional)null), out p3);
            test(p2.HasValue && p3.HasValue && p2.Value == null && p3.Value == null);

            p1 = new Test.OneOptional(58);
            p2 = initial.opOneOptional(p1, out p3);
            test(p2.Value.a == 58 && p3.Value.a == 58);
            Ice.AsyncResult r = initial.begin_opOneOptional(p1);
            p2 = initial.end_opOneOptional(out p3, r);
            test(p2.Value.a == 58 && p3.Value.a == 58);
            p2 = initial.opOneOptional(p1.Value, out p3);
            test(p2.Value.a == 58 && p3.Value.a == 58);
            r = initial.begin_opOneOptional(p1.Value);
            p2 = initial.end_opOneOptional(out p3, r);
            test(p2.Value.a == 58 && p3.Value.a == 58);

            p2 = initial.opOneOptional(new Ice.Optional<Test.OneOptional>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.Size);
            os.writeObject(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opOneOptional", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.Size));
            ReadObjectCallbackI p2cb = new ReadObjectCallbackI();
            @in.readObject(p2cb);
            test(@in.readOptional(3, Ice.OptionalType.Size));
            ReadObjectCallbackI p3cb = new ReadObjectCallbackI();
            @in.readObject(p3cb);
            @in.endEncapsulation();
            test(((Test.OneOptional)p2cb.obj).a == 58 && ((Test.OneOptional)p3cb.obj).a == 58);

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<Test.OneOptionalPrx> p1 = new Ice.Optional<Test.OneOptionalPrx>();
            Ice.Optional<Test.OneOptionalPrx> p3;
            Ice.Optional<Test.OneOptionalPrx> p2 = initial.opOneOptionalProxy(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opOneOptionalProxy(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opOneOptionalProxy(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opOneOptionalProxy(null, out p3);
            test(p2.HasValue && p3.HasValue && p2.Value == null && p3.Value == null);

            //
            // Not allowed by C# language spec because OptionalOnePrx is an interface.
            //
            //p1 = Test.OneOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test"));
            p1 = new Ice.Optional<Test.OneOptionalPrx>(
                Test.OneOptionalPrxHelper.uncheckedCast(communicator.stringToProxy("test")));
            p2 = initial.opOneOptionalProxy(p1, out p3);
            test(p2.Value.Equals(p1.Value) && p3.Value.Equals(p1.Value));

            Ice.AsyncResult r = initial.begin_opOneOptionalProxy(p1);
            p2 = initial.end_opOneOptionalProxy(out p3, r);
            test(p2.Value.Equals(p1.Value) && p3.Value.Equals(p1.Value));
            //p2 = initial.opOneOptionalProxy(p1.Value, out p3);
            //test(p2.Value.Equals(p1.Value) && p3.Value.Equals(p1.Value));
            //r = initial.begin_opOneOptionalProxy(p1.Value);
            //p2 = initial.end_opOneOptionalProxy(out p3, r);
            //test(p2.Value.Equals(p1.Value) && p3.Value.Equals(p1.Value));

            p2 = initial.opOneOptionalProxy(new Ice.Optional<Test.OneOptionalPrx>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.FSize);
            os.startSize();
            os.writeProxy(p1.Value);
            os.endSize();
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opOneOptionalProxy", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.FSize));
            @in.skip(4);
            test(@in.readProxy().Equals(p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.FSize));
            @in.skip(4);
            test(@in.readProxy().Equals(p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<byte[]> p1 = new Ice.Optional<byte[]>();
            Ice.Optional<byte[]> p3;
            Ice.Optional<byte[]> p2 = initial.opByteSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opByteSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opByteSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new byte[100];
            Populate(p1.Value, (byte)56);
            p2 = initial.opByteSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opByteSeq(p1);
            p2 = initial.end_opByteSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opByteSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opByteSeq(p1.Value);
            p2 = initial.end_opByteSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opByteSeq(new Ice.Optional<byte[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeByteSeq(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opByteSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            test(ArraysEqual(@in.readByteSeq(), p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            test(ArraysEqual(@in.readByteSeq(), p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<bool[]> p1 = new Ice.Optional<bool[]>();
            Ice.Optional<bool[]> p3;
            Ice.Optional<bool[]> p2 = initial.opBoolSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opBoolSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opBoolSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new bool[100];
            Populate(p1.Value, true);
            p2 = initial.opBoolSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opBoolSeq(p1);
            p2 = initial.end_opBoolSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opBoolSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opBoolSeq(p1.Value);
            p2 = initial.end_opBoolSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opBoolSeq(new Ice.Optional<bool[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeBoolSeq(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opBoolSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            test(ArraysEqual(@in.readBoolSeq(), p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            test(ArraysEqual(@in.readBoolSeq(), p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<short[]> p1 = new Ice.Optional<short[]>();
            Ice.Optional<short[]> p3;
            Ice.Optional<short[]> p2 = initial.opShortSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opShortSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opShortSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new short[100];
            Populate(p1.Value, (short)56);
            p2 = initial.opShortSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opShortSeq(p1);
            p2 = initial.end_opShortSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opShortSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opShortSeq(p1.Value);
            p2 = initial.end_opShortSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opShortSeq(new Ice.Optional<short[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(p1.Value.Length * 2 + (p1.Value.Length > 254 ? 5 : 1));
            os.writeShortSeq(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opShortSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            test(ArraysEqual(@in.readShortSeq(), p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            test(ArraysEqual(@in.readShortSeq(), p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<int[]> p1 = new Ice.Optional<int[]>();
            Ice.Optional<int[]> p3;
            Ice.Optional<int[]> p2 = initial.opIntSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opIntSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opIntSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new int[100];
            Populate(p1.Value, 56);
            p2 = initial.opIntSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opIntSeq(p1);
            p2 = initial.end_opIntSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opIntSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opIntSeq(p1.Value);
            p2 = initial.end_opIntSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opIntSeq(new Ice.Optional<int[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(p1.Value.Length * 4 + (p1.Value.Length > 254 ? 5 : 1));
            os.writeIntSeq(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opIntSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            test(ArraysEqual(@in.readIntSeq(), p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            test(ArraysEqual(@in.readIntSeq(), p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<long[]> p1 = new Ice.Optional<long[]>();
            Ice.Optional<long[]> p3;
            Ice.Optional<long[]> p2 = initial.opLongSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opLongSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opLongSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new long[100];
            Populate(p1.Value, 56);
            p2 = initial.opLongSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opLongSeq(p1);
            p2 = initial.end_opLongSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opLongSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opLongSeq(p1.Value);
            p2 = initial.end_opLongSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opLongSeq(new Ice.Optional<long[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(p1.Value.Length * 8 + (p1.Value.Length > 254 ? 5 : 1));
            os.writeLongSeq(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opLongSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            test(ArraysEqual(@in.readLongSeq(), p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            test(ArraysEqual(@in.readLongSeq(), p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<float[]> p1 = new Ice.Optional<float[]>();
            Ice.Optional<float[]> p3;
            Ice.Optional<float[]> p2 = initial.opFloatSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opFloatSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opFloatSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new float[100];
            Populate(p1.Value, (float)1.0);
            p2 = initial.opFloatSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opFloatSeq(p1);
            p2 = initial.end_opFloatSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opFloatSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opFloatSeq(p1.Value);
            p2 = initial.end_opFloatSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opFloatSeq(new Ice.Optional<float[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(p1.Value.Length * 4 + (p1.Value.Length > 254 ? 5 : 1));
            os.writeFloatSeq(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opFloatSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            test(ArraysEqual(@in.readFloatSeq(), p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            test(ArraysEqual(@in.readFloatSeq(), p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<double[]> p1 = new Ice.Optional<double[]>();
            Ice.Optional<double[]> p3;
            Ice.Optional<double[]> p2 = initial.opDoubleSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opDoubleSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opDoubleSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new double[100];
            Populate(p1.Value, 1.0);
            p2 = initial.opDoubleSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opDoubleSeq(p1);
            p2 = initial.end_opDoubleSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opDoubleSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opDoubleSeq(p1.Value);
            p2 = initial.end_opDoubleSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opDoubleSeq(new Ice.Optional<double[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(p1.Value.Length * 8 + (p1.Value.Length > 254 ? 5 : 1));
            os.writeDoubleSeq(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opDoubleSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            test(ArraysEqual(@in.readDoubleSeq(), p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            test(ArraysEqual(@in.readDoubleSeq(), p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<String[]> p1 = new Ice.Optional<String[]>();
            Ice.Optional<String[]> p3;
            Ice.Optional<String[]> p2 = initial.opStringSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opStringSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opStringSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new String[10];
            Populate(p1.Value, "test1");
            p2 = initial.opStringSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opStringSeq(p1);
            p2 = initial.end_opStringSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opStringSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opStringSeq(p1.Value);
            p2 = initial.end_opStringSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opStringSeq(new Ice.Optional<String[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.FSize);
            os.startSize();
            os.writeStringSeq(p1.Value);
            os.endSize();
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opStringSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.FSize));
            @in.skip(4);
            test(ArraysEqual(@in.readStringSeq(), p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.FSize));
            @in.skip(4);
            test(ArraysEqual(@in.readStringSeq(), p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<Test.SmallStruct[]> p1 = new Ice.Optional<Test.SmallStruct[]>();
            Ice.Optional<Test.SmallStruct[]> p3;
            Ice.Optional<Test.SmallStruct[]> p2 = initial.opSmallStructSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opSmallStructSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opSmallStructSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new Test.SmallStruct[10];
            for(int i = 0; i < p1.Value.Length; ++i)
            {
                p1.Value[i] = new Test.SmallStruct();
            }
            p2 = initial.opSmallStructSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opSmallStructSeq(p1);
            p2 = initial.end_opSmallStructSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opSmallStructSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opSmallStructSeq(p1.Value);
            p2 = initial.end_opSmallStructSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opSmallStructSeq(new Ice.Optional<Test.SmallStruct[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(p1.Value.Length + (p1.Value.Length > 254 ? 5 : 1));
            Test.SmallStructSeqHelper.write(os, p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opSmallStructSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            Test.SmallStruct[] arr = Test.SmallStructSeqHelper.read(@in);
            test(ArraysEqual(arr, p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            arr = Test.SmallStructSeqHelper.read(@in);
            test(ArraysEqual(arr, p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<List<Test.SmallStruct>> p1 = new Ice.Optional<List<Test.SmallStruct>>();
            Ice.Optional<List<Test.SmallStruct>> p3;
            Ice.Optional<List<Test.SmallStruct>> p2 = initial.opSmallStructList(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opSmallStructList(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opSmallStructList(null, out p3);
            test(p2.HasValue && p2.Value.Count == 0 && p3.HasValue && p3.Value.Count == 0);

            p1 = new List<Test.SmallStruct>();
            for(int i = 0; i < 10; ++i)
            {
                p1.Value.Add(new Test.SmallStruct());
            }
            p2 = initial.opSmallStructList(p1, out p3);
            test(ListsEqual(p2.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opSmallStructList(p1);
            p2 = initial.end_opSmallStructList(out p3, r);
            test(ListsEqual(p2.Value, p1.Value));
            p2 = initial.opSmallStructList(p1.Value, out p3);
            test(ListsEqual(p2.Value, p1.Value));
            r = initial.begin_opSmallStructList(p1.Value);
            p2 = initial.end_opSmallStructList(out p3, r);
            test(ListsEqual(p2.Value, p1.Value));

            p2 = initial.opSmallStructList(new Ice.Optional<List<Test.SmallStruct>>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(p1.Value.Count + (p1.Value.Count > 254 ? 5 : 1));
            Test.SmallStructListHelper.write(os, p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opSmallStructList", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            List<Test.SmallStruct> arr = Test.SmallStructListHelper.read(@in);
            test(ListsEqual(arr, p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            arr = Test.SmallStructListHelper.read(@in);
            test(ListsEqual(arr, p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<Test.FixedStruct[]> p1 = new Ice.Optional<Test.FixedStruct[]>();
            Ice.Optional<Test.FixedStruct[]> p3;
            Ice.Optional<Test.FixedStruct[]> p2 = initial.opFixedStructSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opFixedStructSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opFixedStructSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new Test.FixedStruct[10];
            for(int i = 0; i < p1.Value.Length; ++i)
            {
                p1.Value[i] = new Test.FixedStruct();
            }
            p2 = initial.opFixedStructSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opFixedStructSeq(p1);
            p2 = initial.end_opFixedStructSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opFixedStructSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opFixedStructSeq(p1.Value);
            p2 = initial.end_opFixedStructSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opFixedStructSeq(new Ice.Optional<Test.FixedStruct[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(p1.Value.Length * 4 + (p1.Value.Length > 254 ? 5 : 1));
            Test.FixedStructSeqHelper.write(os, p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opFixedStructSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            Test.FixedStruct[] arr = Test.FixedStructSeqHelper.read(@in);
            test(ArraysEqual(arr, p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            arr = Test.FixedStructSeqHelper.read(@in);
            test(ArraysEqual(arr, p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<LinkedList<Test.FixedStruct>> p1 = new Ice.Optional<LinkedList<Test.FixedStruct>>();
            Ice.Optional<LinkedList<Test.FixedStruct>> p3;
            Ice.Optional<LinkedList<Test.FixedStruct>> p2 = initial.opFixedStructList(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opFixedStructList(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opFixedStructList(null, out p3);
            test(p2.HasValue && p2.Value.Count == 0 && p3.HasValue && p3.Value.Count == 0);

            p1 = new LinkedList<Test.FixedStruct>();
            for(int i = 0; i < 10; ++i)
            {
                p1.Value.AddLast(new Test.FixedStruct());
            }
            p2 = initial.opFixedStructList(p1, out p3);
            test(ListsEqual(p2.Value, p1.Value) && ListsEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opFixedStructList(p1);
            p2 = initial.end_opFixedStructList(out p3, r);
            test(ListsEqual(p2.Value, p1.Value) && ListsEqual(p3.Value, p1.Value));
            p2 = initial.opFixedStructList(p1.Value, out p3);
            test(ListsEqual(p2.Value, p1.Value) && ListsEqual(p3.Value, p1.Value));
            r = initial.begin_opFixedStructList(p1.Value);
            p2 = initial.end_opFixedStructList(out p3, r);
            test(ListsEqual(p2.Value, p1.Value) && ListsEqual(p3.Value, p1.Value));

            p2 = initial.opFixedStructList(new Ice.Optional<LinkedList<Test.FixedStruct>>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(p1.Value.Count * 4 + (p1.Value.Count > 254 ? 5 : 1));
            Test.FixedStructListHelper.write(os, p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opFixedStructList", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            LinkedList<Test.FixedStruct> arr = Test.FixedStructListHelper.read(@in);
            test(ListsEqual(arr, p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            arr = Test.FixedStructListHelper.read(@in);
            test(ListsEqual(arr, p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<Test.VarStruct[]> p1 = new Ice.Optional<Test.VarStruct[]>();
            Ice.Optional<Test.VarStruct[]> p3;
            Ice.Optional<Test.VarStruct[]> p2 = initial.opVarStructSeq(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opVarStructSeq(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opVarStructSeq(null, out p3);
            test(p2.HasValue && p2.Value.Length == 0 && p3.HasValue && p3.Value.Length == 0);

            p1 = new Test.VarStruct[10];
            for(int i = 0; i < p1.Value.Length; ++i)
            {
                p1.Value[i] = new Test.VarStruct("");
            }
            p2 = initial.opVarStructSeq(p1, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opVarStructSeq(p1);
            p2 = initial.end_opVarStructSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            p2 = initial.opVarStructSeq(p1.Value, out p3);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));
            r = initial.begin_opVarStructSeq(p1.Value);
            p2 = initial.end_opVarStructSeq(out p3, r);
            test(ArraysEqual(p2.Value, p1.Value) && ArraysEqual(p3.Value, p1.Value));

            p2 = initial.opVarStructSeq(new Ice.Optional<Test.VarStruct[]>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.FSize);
            os.startSize();
            Test.VarStructSeqHelper.write(os, p1.Value);
            os.endSize();
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opVarStructSeq", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.FSize));
            @in.skip(4);
            Test.VarStruct[] arr = Test.VarStructSeqHelper.read(@in);
            test(ArraysEqual(arr, p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.FSize));
            @in.skip(4);
            arr = Test.VarStructSeqHelper.read(@in);
            test(ArraysEqual(arr, p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

#if !SILVERLIGHT
        {
            Ice.Optional<Test.SerializableClass> p1 = new Ice.Optional<Test.SerializableClass>();
            Ice.Optional<Test.SerializableClass> p3;
            Ice.Optional<Test.SerializableClass> p2 = initial.opSerializable(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opSerializable(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);

            p1 = new Test.SerializableClass(58);
            p2 = initial.opSerializable(p1, out p3);
            test(p2.Value.Equals(p1.Value) && p3.Value.Equals(p1.Value));
            Ice.AsyncResult r = initial.begin_opSerializable(p1);
            p2 = initial.end_opSerializable(out p3, r);
            test(p2.Value.Equals(p1.Value) && p3.Value.Equals(p1.Value));
            p2 = initial.opSerializable(p1.Value, out p3);
            test(p2.Value.Equals(p1.Value) && p3.Value.Equals(p1.Value));
            r = initial.begin_opSerializable(p1.Value);
            p2 = initial.end_opSerializable(out p3, r);
            test(p2.Value.Equals(p1.Value) && p3.Value.Equals(p1.Value));

            p2 = initial.opSerializable(new Ice.Optional<Test.SerializableClass>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSerializable(p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opSerializable", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            Test.SerializableClass sc = Test.SerializableHelper.read(@in);
            test(sc.Equals(p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            sc = Test.SerializableHelper.read(@in);
            test(sc.Equals(p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }
#endif

        {
            Ice.Optional<Dictionary<int, int>> p1 = new Ice.Optional<Dictionary<int, int>>();
            Ice.Optional<Dictionary<int, int>> p3;
            Ice.Optional<Dictionary<int, int>> p2 = initial.opIntIntDict(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opIntIntDict(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opIntIntDict(null, out p3);
            test(p2.HasValue && p2.Value.Count == 0 && p3.HasValue && p3.Value.Count == 0);

            p1 = new Dictionary<int, int>();
            p1.Value.Add(1, 2);
            p1.Value.Add(2, 3);
            p2 = initial.opIntIntDict(p1, out p3);
            test(MapsEqual(p2.Value, p1.Value) && MapsEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opIntIntDict(p1);
            p2 = initial.end_opIntIntDict(out p3, r);
            test(MapsEqual(p2.Value, p1.Value) && MapsEqual(p3.Value, p1.Value));
            p2 = initial.opIntIntDict(p1.Value, out p3);
            test(MapsEqual(p2.Value, p1.Value) && MapsEqual(p3.Value, p1.Value));
            r = initial.begin_opIntIntDict(p1.Value);
            p2 = initial.end_opIntIntDict(out p3, r);
            test(MapsEqual(p2.Value, p1.Value) && MapsEqual(p3.Value, p1.Value));

            p2 = initial.opIntIntDict(new Ice.Optional<Dictionary<int, int>>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.VSize);
            os.writeSize(p1.Value.Count * 8 + (p1.Value.Count > 254 ? 5 : 1));
            Test.IntIntDictHelper.write(os, p1.Value);
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opIntIntDict", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.VSize));
            @in.skipSize();
            Dictionary<int, int> m = Test.IntIntDictHelper.read(@in);
            test(MapsEqual(m, p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.VSize));
            @in.skipSize();
            m = Test.IntIntDictHelper.read(@in);
            test(MapsEqual(m, p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }

        {
            Ice.Optional<Dictionary<string, int>> p1 = new Ice.Optional<Dictionary<string, int>>();
            Ice.Optional<Dictionary<string, int>> p3;
            Ice.Optional<Dictionary<string, int>> p2 = initial.opStringIntDict(p1, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opStringIntDict(Ice.Util.None, out p3);
            test(!p2.HasValue && !p3.HasValue);
            p2 = initial.opStringIntDict(null, out p3);
            test(p2.HasValue && p2.Value.Count == 0 && p3.HasValue && p3.Value.Count == 0);

            p1 = new Dictionary<string, int>();
            p1.Value.Add("1", 1);
            p1.Value.Add("2", 2);
            p2 = initial.opStringIntDict(p1, out p3);
            test(MapsEqual(p2.Value, p1.Value) && MapsEqual(p3.Value, p1.Value));
            Ice.AsyncResult r = initial.begin_opStringIntDict(p1);
            p2 = initial.end_opStringIntDict(out p3, r);
            test(MapsEqual(p2.Value, p1.Value) && MapsEqual(p3.Value, p1.Value));
            p2 = initial.opStringIntDict(p1.Value, out p3);
            test(MapsEqual(p2.Value, p1.Value) && MapsEqual(p3.Value, p1.Value));
            r = initial.begin_opStringIntDict(p1.Value);
            p2 = initial.end_opStringIntDict(out p3, r);
            test(MapsEqual(p2.Value, p1.Value) && MapsEqual(p3.Value, p1.Value));

            p2 = initial.opStringIntDict(new Ice.Optional<Dictionary<string, int>>(), out p3);
            test(!p2.HasValue && !p3.HasValue); // Ensure out parameter is cleared.

            os = Ice.Util.createOutputStream(communicator);
            os.startEncapsulation();
            os.writeOptional(2, Ice.OptionalType.FSize);
            os.startSize();
            Test.StringIntDictHelper.write(os, p1.Value);
            os.endSize();
            os.endEncapsulation();
            inEncaps = os.finished();
            initial.ice_invoke("opStringIntDict", Ice.OperationMode.Normal, inEncaps, out outEncaps);
            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            test(@in.readOptional(1, Ice.OptionalType.FSize));
            @in.skip(4);
            Dictionary<string, int> m = Test.StringIntDictHelper.read(@in);
            test(MapsEqual(m, p1.Value));
            test(@in.readOptional(3, Ice.OptionalType.FSize));
            @in.skip(4);
            m = Test.StringIntDictHelper.read(@in);
            test(MapsEqual(m, p1.Value));
            @in.endEncapsulation();

            @in = Ice.Util.createInputStream(communicator, outEncaps);
            @in.startEncapsulation();
            @in.endEncapsulation();
        }
        WriteLine("ok");

        Write("testing exception optionals... ");
        Flush();
        {
            try
            {
                Ice.Optional<int> a = new Ice.Optional<int>();
                Ice.Optional<string> b = new Ice.Optional<string>();
                Ice.Optional<Test.OneOptional> o = new Ice.Optional<Test.OneOptional>();
                initial.opOptionalException(a, b, o);
            }
            catch(Test.OptionalException ex)
            {
                test(!ex.hasA);
                test(!ex.hasB);
                test(!ex.hasO);
            }

            try
            {
                Ice.Optional<int> a = new Ice.Optional<int>(30);
                Ice.Optional<string> b = new Ice.Optional<string>("test");
                Ice.Optional<Test.OneOptional> o = new Ice.Optional<Test.OneOptional>(new Test.OneOptional(53));
                initial.opOptionalException(a, b, o);
            }
            catch(Test.OptionalException ex)
            {
                test(ex.a == 30);
                test(ex.b.Equals("test"));
                test(ex.o.a == 53);
            }
        }
        WriteLine("ok");

#if SILVERLIGHT
        initial.shutdown();
#else
        return initial;
#endif
    }

    internal static bool ArraysEqual<T>(T[] a1, T[] a2)
    {
        if(ReferenceEquals(a1, a2))
        {
            return true;
        }

        if(a1 == null || a2 == null)
        {
            return false;
        }

        if(a1.Length != a2.Length)
        {
            return false;
        }

        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for(int i = 0; i < a1.Length; ++i)
        {
            if(!comparer.Equals(a1[i], a2[i]))
            {
                return false;
            }
        }

        return true;
    }

    internal static bool ListsEqual<T>(ICollection<T> a1, ICollection<T> a2)
    {
        if(ReferenceEquals(a1, a2))
        {
            return true;
        }

        if(a1 == null || a2 == null)
        {
            return false;
        }

        if(a1.Count != a2.Count)
        {
            return false;
        }

        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        IEnumerator<T> a1i = a1.GetEnumerator();
        IEnumerator<T> a2i = a2.GetEnumerator();
        while(a1i.MoveNext() && a2i.MoveNext())
        {
            if(!comparer.Equals(a1i.Current, a2i.Current))
            {
                return false;
            }
        }

        return true;
    }

    internal static bool MapsEqual<K, V>(Dictionary<K, V> d1, Dictionary<K, V> d2)
    {
        if(ReferenceEquals(d1, d2))
        {
            return true;
        }

        if(d1 == null || d2 == null)
        {
            return false;
        }

        if(d1.Count != d2.Count)
        {
            return false;
        }

        EqualityComparer<V> valueComparer = EqualityComparer<V>.Default;
        foreach(K key in d1.Keys)
        {
            if(!d2.ContainsKey(key) || !valueComparer.Equals(d1[key], d2[key]))
            {
                return false;
            }
        }

        return true;
    }

    internal static void Populate<T>(T[] arr, T value)
    {
        for(int i = 0; i < arr.Length; ++i)
        {
            arr[i] = value;
        }
    }


    private class TestObjectReader : Ice.ObjectReader
    {
        public override void read(Ice.InputStream @in)
        {
            @in.startObject();
            @in.startSlice();
            @in.endSlice();
            @in.endObject(false);
        }
    }

    private class BObjectReader : Ice.ObjectReader
    {
        public override void read(Ice.InputStream @in)
        {
            @in.startObject();
            // ::Test::B
            @in.startSlice();
            @in.readInt();
            @in.endSlice();
            // ::Test::A
            @in.startSlice();
            @in.readInt();
            @in.endSlice();
            @in.endObject(false);
        }
    }

    private class CObjectReader : Ice.ObjectReader
    {
        public override void read(Ice.InputStream @in)
        {
            @in.startObject();
            // ::Test::C
            @in.startSlice();
            @in.skipSlice();
            // ::Test::B
            @in.startSlice();
            @in.readInt();
            @in.endSlice();
            // ::Test::A
            @in.startSlice();
            @in.readInt();
            @in.endSlice();
            @in.endObject(false);
        }
    }

    private class DObjectWriter : Ice.ObjectWriter
    {
        public override void write(Ice.OutputStream @out)
        {
            @out.startObject(null);
            // ::Test::D
            @out.startSlice("::Test::D", false);
            string s = "test";
            @out.writeString(s);
            @out.writeOptional(1, Ice.OptionalType.FSize);
            string[] o = { "test1", "test2", "test3", "test4" };
            @out.startSize();
            @out.writeStringSeq(o);
            @out.endSize();
            Test.A a = new Test.A();
            a.mc = 18;
            @out.writeOptional(1000, Ice.OptionalType.Size);
            @out.writeObject(a);
            @out.endSlice();
            // ::Test::B
            @out.startSlice(Test.B.ice_staticId(), false);
            int v = 14;
            @out.writeInt(v);
            @out.endSlice();
            // ::Test::A
            @out.startSlice(Test.A.ice_staticId(), true);
            @out.writeInt(v);
            @out.endSlice();
            @out.endObject();
        }
    }

    private class DObjectReader : Ice.ObjectReader
    {
        public override void read(Ice.InputStream @in)
        {
            @in.startObject();
            // ::Test::D
            @in.startSlice();
            string s = @in.readString();
            test(s.Equals("test"));
            test(@in.readOptional(1, Ice.OptionalType.FSize));
            @in.skip(4);
            string[] o = @in.readStringSeq();
            test(o.Length == 4 &&
                 o[0].Equals("test1") && o[1].Equals("test2") && o[2].Equals("test3") && o[3].Equals("test4"));
            test(@in.readOptional(1000, Ice.OptionalType.Size));
            @in.readObject(a);
            @in.endSlice();
            // ::Test::B
            @in.startSlice();
            @in.readInt();
            @in.endSlice();
            // ::Test::A
            @in.startSlice();
            @in.readInt();
            @in.endSlice();
            @in.endObject(false);
        }

        internal void check()
        {
            test(((Test.A)a.obj).mc == 18);
        }

        private ReadObjectCallbackI a = new ReadObjectCallbackI();
    }

    private class FactoryI : Ice.ObjectFactory
    {
        public Ice.Object create(string typeId)
        {
            if(!_enabled)
            {
                return null;
            }

            if(typeId.Equals(Test.OneOptional.ice_staticId()))
            {
                return new TestObjectReader();
            }
            else if(typeId.Equals(Test.MultiOptional.ice_staticId()))
            {
                return new TestObjectReader();
            }
            else if(typeId.Equals(Test.B.ice_staticId()))
            {
                return new BObjectReader();
            }
            else if(typeId.Equals(Test.C.ice_staticId()))
            {
                return new CObjectReader();
            }
            else if(typeId.Equals("::Test::D"))
            {
                return new DObjectReader();
            }

            return null;
        }

        public void destroy()
        {
        }

        internal void setEnabled(bool enabled)
        {
            _enabled = enabled;
        }

        private bool _enabled;
    }

    private class ReadObjectCallbackI : Ice.ReadObjectCallback
    {
        public void invoke(Ice.Object obj)
        {
            this.obj = obj;
        }

        internal Ice.Object obj;
    }
}