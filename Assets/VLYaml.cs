﻿/*
VLYaml.cs is part of the VLAB project.
Copyright (c) 2017 Li Alex Zhang and Contributors

Permission is hereby granted, free of charge, to any person obtaining a 
copy of this software and associated documentation files (the "Software"),
to deal in the Software without restriction, including without limitation
the rights to use, copy, modify, merge, publish, distribute, sublicense,
and/or sell copies of the Software, and to permit persons to whom the 
Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included 
in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Runtime.InteropServices;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;

namespace VLab
{
    public class VLabYamlConverter : IYamlTypeConverter
    {
        public bool istag = true;

        public bool Accepts(Type type)
        {
            if (type == typeof(Param) || type == typeof(Vector3) || type == typeof(Color))
            {
                return true;
            }
            return false;
        }

        public object ReadYaml(IParser parser, Type type)
        {
            object o;
            var e = (Scalar)parser.Current;
            if (type == typeof(Param))
            {
                var t = e.Tag.Substring(e.Tag.LastIndexOf(':') + 1).Convert<ParamType>();
                var v = e.Value.Convert(t);
                o = new Param(t, v);
            }
            else
            {
                o = e.Value.Convert(type);
            }
            parser.MoveNext();
            return o;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            Scalar e;
            if (type == typeof(Param))
            {
                var v = (Param)value;
                if (istag)
                {
                    e = new Scalar(null, "tag:yaml.org,2002:" + v.Type.ToString(), v.Value.Convert<string>(), ScalarStyle.Plain, false, false);
                }
                else
                {
                    e = new Scalar(v.Value.Convert<string>());
                }
            }
            else
            {
                e = new Scalar(value.Convert<string>());
            }
            emitter.Emit(e);
        }
    }

    public static class Yaml
    {
        static Serializer serializer = new Serializer(SerializationOptions.DisableAliases | SerializationOptions.EmitDefaults);
        static Deserializer deserializer = new Deserializer(ignoreUnmatched: true);
        static VLabYamlConverter vlabyamlconverter = new VLabYamlConverter();

        static Yaml()
        {
            serializer.RegisterTypeConverter(vlabyamlconverter);
            deserializer.RegisterTypeConverter(vlabyamlconverter);
        }

        public static void WriteYaml<T>(string path, T data, bool isvlabtag = true)
        {
            File.WriteAllText(path, Serialize(data, isvlabtag));
        }

        public static string Serialize<T>(T data, bool isvlabtag = true)
        {
            var s = new StringBuilder();
            var old = vlabyamlconverter.istag;
            vlabyamlconverter.istag = isvlabtag;
            serializer.Serialize(new StringWriter(s), data);
            vlabyamlconverter.istag = old;
            return s.ToString();
        }

        public static T ReadYaml<T>(string path)
        {
            using (var s = new StringReader(File.ReadAllText(path)))
            {
                return deserializer.Deserialize<T>(s);
            }
        }

        public static T Deserialize<T>(string data)
        {
            return deserializer.Deserialize<T>(new StringReader(data));
        }
    }

}