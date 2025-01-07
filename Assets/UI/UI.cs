/*
UI.cs is part of the Experica.
Copyright (c) 2016 Li Alex Zhang and Contributors

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
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using System.Linq;
using Experica.NetEnv;
using UnityEngine.Rendering;
using Unity.Properties;
using Unity.Collections;

namespace Experica.Environment
{
    public class UI : MonoBehaviour
    {
        public AppManager appmgr;
        public UIDocument uidoc;
        public VisualTreeAsset ToggleString, ToggleEnum, ToggleBool, ToggleInteger, ToggleUInteger, ToggleFloat, ToggleDouble, ToggleVector2, ToggleVector3, ToggleVector4,
            ParamString, ParamEnum, ParamBool, ParamInteger, ParamUInteger, ParamFloat, ParamDouble, ParamVector2, ParamVector3, ParamVector4,
            ExtendButton, ParamsFoldout, AboutWindow, ConfigWindow, AddConfigParamWindow;

        VisualElement root, mainmenu, maincontent;
        public Toggle client;
        public Button addconfigextendparam;


        void OnEnable()
        {
            root = uidoc.rootVisualElement;
            root.RegisterCallback<GeometryChangedEvent>(e => appmgr.OnScreenSizeChanged());
            mainmenu = root.Q("MainMenu");
            maincontent = root.Q("MainContent");
            // Main Menu
            mainmenu.Q<Button>("About").RegisterCallback<ClickEvent>(e => OnAboutWindow(root));
            mainmenu.Q<Button>("Config").RegisterCallback<ClickEvent>(e => OnConfigWindow(root));
            client = mainmenu.Q<Toggle>("Client");
            client.RegisterValueChangedCallback(e => appmgr.OnClientChanged(e.newValue));
        }

        void OnAboutWindow(VisualElement parent)
        {
            var w = AboutWindow.Instantiate()[0];
            w.Q<Label>("Product").text = Application.productName;
            w.Q<Label>("ProductVersion").text = Application.version;
            w.Q<Label>("UnityVersion").text = Application.unityVersion;
            w.Q<Button>("Close").RegisterCallback<ClickEvent>(e => parent.Remove(w));

            w.style.position = Position.Absolute;
            w.style.top = Length.Percent(25);
            w.style.left = Length.Percent(25);
            w.style.width = Length.Percent(50);
            w.style.height = Length.Percent(50);
            parent.Add(w);
        }

        void OnConfigWindow(VisualElement parent)
        {
            var w = ConfigWindow.Instantiate()[0];
            w.Q<Button>("Close").RegisterCallback<ClickEvent>(e => parent.Remove(w));
            var cfgcontent = w.Q<ScrollView>("Content");
            UpdateConfig(appmgr.cfgmgr.config, cfgcontent);

            w.style.position = Position.Absolute;
            w.style.top = Length.Percent(20);
            w.style.left = Length.Percent(25);
            w.style.width = Length.Percent(50);
            w.style.height = Length.Percent(60);
            parent.Add(w);
        }

        public void UpdateConfig(EnvironmentConfig config, ScrollView content)
        {
            content.Clear();
            var previousui = content.Children().ToList();
            var previousuiname = previousui.Select(i => i.name).ToList();
            // since ExtendParam is a param container and we always show them, so here we do not AddParamUI for the container itself, but add its content
            var currentpropertyname = config.properties.Keys.Where(i => i != "ExtendParam").ToArray();
            var ui2update = previousuiname.Intersect(currentpropertyname);
            var ui2remove = previousuiname.Except(currentpropertyname);
            var ui2add = currentpropertyname.Except(previousuiname);

            if (ui2update.Count() > 0)
            {
                foreach (var p in ui2update)
                {
                    var ui = previousui[previousuiname.IndexOf(p)];
                    var namelabel = ui.Q<Label>("Name");
                    namelabel.text = p;
                    var vi = ui.Q("Value");
                    var ds = config.properties[p];
                    var db = vi.GetBinding("value") as DataBinding;
                    db.dataSource = ds;
                    ds.NotifyValue();
                }
            }
            if (ui2remove.Count() > 0)
            {
                foreach (var p in ui2remove)
                {
                    content.Remove(previousui[previousuiname.IndexOf(p)]);
                }
            }
            if (ui2add.Count() > 0)
            {
                foreach (var p in ui2add)
                {
                    AddParamUI(p, p, config.properties[p], false, null, content);
                }
            }

            foreach (var p in config.extendproperties.Keys.ToArray())
            {
                AddParamUI(p, p, config.extendproperties[p], false, null, content, config, true);
            }
            content.scrollOffset = content.contentRect.size;
        }

        void AddParamUI<T>(string id, string name, IDataSource<T> source, bool isinherit, Action<string, bool> inherithandler, VisualElement parent, DataClass datasourceclass = null, bool isextendparam = false)
        {
            AddParamUI(id, name, source.Type, source.Value, isinherit, inherithandler, parent, datasourceclass, source, "Value", isextendparam);
        }

        void AddParamUI(string id, string name, Type T, object value, bool isinherit, Action<string, bool> inherithandler, VisualElement parent, DataClass datasourceclass = null, object datasource = null, string datapath = "Value", bool isextendparam = false)
        {
            VisualElement ui, valueinput;

            if (T.IsEnum)
            {
                var asset = inherithandler == null ? ParamEnum : ToggleEnum;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<EnumField>("Value");
                vi.Init((Enum)value);
                valueinput = vi;
            }
            else if (T == typeof(bool))
            {
                var asset = inherithandler == null ? ParamBool : ToggleBool;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<Toggle>("Value");
                vi.value = (bool)value;
                vi.label = vi.value ? "True" : "False";
                vi.RegisterValueChangedCallback(e => vi.label = e.newValue ? "True" : "False");
                valueinput = vi;
            }
            else if (T == typeof(int))
            {
                var asset = inherithandler == null ? ParamInteger : ToggleInteger;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<IntegerField>("Value");
                vi.value = (int)value;
                valueinput = vi;
            }
            else if (T == typeof(uint))
            {
                var asset = inherithandler == null ? ParamUInteger : ToggleUInteger;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<UnsignedIntegerField>("Value");
                vi.value = (uint)value;
                valueinput = vi;
            }
            else if (T == typeof(float))
            {
                var asset = inherithandler == null ? ParamFloat : ToggleFloat;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<FloatField>("Value");
                vi.value = (float)value;
                valueinput = vi;
            }
            else if (T == typeof(double))
            {
                var asset = inherithandler == null ? ParamDouble : ToggleDouble;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<DoubleField>("Value");
                vi.value = (double)value;
                valueinput = vi;
            }
            else if (T == typeof(Vector2))
            {
                var asset = inherithandler == null ? ParamVector2 : ToggleVector2;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<Vector2Field>("Value");
                vi.value = (Vector2)value;
                valueinput = vi;
            }
            else if (T == typeof(Vector3))
            {
                var asset = inherithandler == null ? ParamVector3 : ToggleVector3;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<Vector3Field>("Value");
                vi.value = (Vector3)value;
                valueinput = vi;
            }
            else if (T == typeof(Vector4))
            {
                var asset = inherithandler == null ? ParamVector4 : ToggleVector4;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<Vector4Field>("Value");
                vi.value = (Vector4)value;
                valueinput = vi;
            }
            else if (T == typeof(Color))
            {
                var asset = inherithandler == null ? ParamVector4 : ToggleVector4;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<Vector4Field>("Value");
                vi.value = (Color)value;
                valueinput = vi;
            }
            else
            {
                var asset = inherithandler == null ? ParamString : ToggleString;
                ui = asset.Instantiate()[0];
                ui.name = id;

                var vi = ui.Q<TextField>("Value");
                vi.value = value.Convert<string>(T);
                valueinput = vi;
            }

            if (inherithandler == null)
            {
                ui.Q<Button>("Name").text = name;
            }
            else
            {
                var nametoggle = ui.Q<Toggle>("Name");
                nametoggle.label = name;
                nametoggle.value = isinherit;
                nametoggle.RegisterValueChangedCallback(e => inherithandler(id, e.newValue));
            }

            if (datasource != null)
            {
                var binding = new DataBinding
                {
                    dataSource = datasource,
                    dataSourcePath = new PropertyPath(datapath),
                };
                if (T == typeof(Color))
                {
                    binding.sourceToUiConverters.AddConverter((ref object s) => { var c = (Color)s; return new Vector4(c.r, c.g, c.b, c.a); });
                    binding.uiToSourceConverters.AddConverter((ref Vector4 v) => (object)new Color(v.x, v.y, v.z, v.w));
                }
                else if (T == typeof(FixedString512Bytes))
                {
                    binding.sourceToUiConverters.AddConverter((ref object s) => s.ToString());
                    binding.uiToSourceConverters.AddConverter((ref string v) => (object)new FixedString512Bytes(v));
                }
                else if (T.IsGenericType && (T.GetGenericTypeDefinition() == typeof(List<>) || T.GetGenericTypeDefinition() == typeof(Dictionary<,>)))
                {
                    binding.sourceToUiConverters.AddConverter((ref object s) => s.Convert<string>(T));
                    binding.uiToSourceConverters.AddConverter((ref string v) => v.Convert(typeof(string), T));
                }
                valueinput.SetBinding("value", binding);
            }
            if (isextendparam && datasourceclass != null)
            {
                var deletebutton = ExtendButton.Instantiate().Q<Button>("Delete");
                deletebutton.RegisterCallback<ClickEvent>(e =>
                {
                    datasourceclass.RemoveExtendProperty(id);
                    parent.Remove(ui);
                });
                ui.Insert(0, deletebutton);
            }
            parent.Add(ui);
        }


        void AddParamsFoldoutUI(string[] ids, string[] names, IDataSource<object>[] sources, bool[] inherits, Action<string, bool> inherithandler, VisualElement parent, string groupname, DataClass datasourceclass = null, bool isextendparam = false)
        {
            var foldout = ParamsFoldout.Instantiate().Q<Foldout>();
            foldout.name = groupname;
            foldout.text = groupname;

            for (int i = 0; i < ids.Length; i++)
            {
                AddParamUI(ids[i], names[i], sources[i], inherits[i], inherithandler, foldout, datasourceclass, isextendparam);
            }
            parent.Add(foldout);
        }

        void OnDisable()
        {

        }

    }
}