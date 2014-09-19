using System;
using System.Linq;
using PluginsCore.Utils;
using PluginsHost;
using Xunit;

namespace PluginsHostTests
{
    public class PluginsCollectionTests
    {
        public const string TEST_PLUGIN1_ASSEMBLY = "Plugin1.dll";
        public const string TEST_PLUGIN1_ASSEMBLY_WITHOUT_EXTENSION = "Plugin1";
        public const string TEST_PLUGIN1_FULLNAME = "Plugin1.PluginOne";

        [Fact]
        public void InitializePluginsCollection()
        {
            var col = new PluginsCollection();
        }

        [Fact]
        public void EmptyPluginsCollectionAtStart()
        {
            var col = new PluginsCollection();
            var startCount = col.Count();
            Assert.Equal(startCount, 0);
        }

        [Fact]
        public void LoadTestPluginByAssemblyAndName()
        {
            var col = new PluginsCollection();
            var plug = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);

            Assert.Equal(col.Count(), 1);
            Assert.NotNull(plug);
        }

        [Fact]
        public void LoadTestPluginByAssemblyWithoutExtensionAndName()
        {
            var col = new PluginsCollection();
            var plug = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY_WITHOUT_EXTENSION, TEST_PLUGIN1_FULLNAME);

            Assert.Equal(col.Count(), 1);
            Assert.NotNull(plug);
        }

        [Fact]
        public void LoadTestPluginByAssembly()
        {
            var col = new PluginsCollection();
            var plug = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY);
            var count = plug.Count();

            Assert.Equal(count, 1);
            Assert.Equal(col.Count(), 1);
        }

        [Fact]
        public void LoadTestPluginByAssemblyWithoutExtension()
        {
            var col = new PluginsCollection();
            var plug = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY_WITHOUT_EXTENSION);

            Assert.Equal(plug.Count(), 1);
            Assert.Equal(col.Count(), 1);
        }

        [Fact]
        public void LoadAndRemoveAllPlugins()
        {
            var col = new PluginsCollection();
            var plug = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);

            Assert.NotNull(plug);
            Assert.True(col.Any());

            col.StopAndRemoveAll();
            Assert.Equal(col.Count(), 0);
        }

        [Fact]
        public void LoadAndRemovePluginAndGCCollect()
        {
            var col = new PluginsCollection();
            var plug = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);

            Assert.NotNull(plug);
            Assert.True(col.Any());

            col.RemovePlugin(plug);
            Assert.Equal(col.Count(), 0);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.Throws<AppDomainUnloadedException>(() => plug.GetFields());
            Assert.Throws<AppDomainUnloadedException>(() => plug.Close());
        }

        [Fact]
        public void LoadAndRemoveAllPluginsAndGCCollect()
        {
            var col = new PluginsCollection();
            var plug1 = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);
            var plug2 = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);

            Assert.NotNull(plug1);
            Assert.NotNull(plug2);
            Assert.Equal(col.Count(), 2);

            col.StopAndRemoveAll();
            Assert.Equal(col.Count(), 0);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.Throws<AppDomainUnloadedException>(() => plug1.GetFields());
            Assert.Throws<AppDomainUnloadedException>(() => plug1.Close());
            Assert.Throws<AppDomainUnloadedException>(() => plug2.GetFields());
            Assert.Throws<AppDomainUnloadedException>(() => plug2.Close());
        }

        [Fact]
        public void LoadAndUsingPluginsWithGCCollect()
        {
            var col = new PluginsCollection();
            var plug1 = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);
            var plug2 = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);

            Assert.NotNull(plug1);
            Assert.NotNull(plug2);
            Assert.Equal(col.Count(), 2);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            Assert.DoesNotThrow(() => plug1.GetFields());
            Assert.DoesNotThrow(plug1.Close);
            Assert.DoesNotThrow(() => plug2.GetFields());
            Assert.DoesNotThrow(plug2.Close);
        }

        [Fact]
        public void LoadTwoSamePlugins()
        {
            var col = new PluginsCollection();
            var plug1 = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);
            var plug2 = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);

            Assert.NotNull(plug1);
            Assert.NotNull(plug2);
            Assert.NotEqual(plug1.GetHashCode(), plug2.GetHashCode());
            Assert.NotEqual(plug1.GetHashCode(), plug2.GetHashCode());
            Assert.Equal(col.Count(), 2);
        }

        [Fact]
        public void LoadedPluginNotInCurrentAppDomain()
        {
            var col = new PluginsCollection();
            var plug1 = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var foundAssembly =
                currentAssemblies.Where(
                    x =>
                        x.FullName == plug1.ToString() ||
                        x.GetType().FullName == plug1.TypeFullName);

            Assert.NotNull(plug1);
            Assert.Equal(foundAssembly.Count(), 0);
        }

        [Fact]
        public void LoadAndSelectPlugin()
        {
            var col = new PluginsCollection();
            var plug1 = col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);
            var plug2 = col[TEST_PLUGIN1_FULLNAME].FirstOrDefault();
            
            Assert.Equal(plug1, plug2);
            Assert.Equal(plug1, plug2);
        }

        [Fact]
        public void LoadAndCheckContainsPlugin()
        {
            var col = new PluginsCollection();
            col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, TEST_PLUGIN1_FULLNAME);

            Assert.True(col.Contains(TEST_PLUGIN1_FULLNAME));
            Assert.False(col.Contains("fsefsefse"));
        }

        [Fact]
        public void LoadPluginByBadAssemblyAndBadName()
        {
            var col = new PluginsCollection();
            Assert.Throws<PluginsException>(() => col.LoadPlugin("badAsm", "badName"));
        }

        [Fact]
        public void LoadPluginByBadAssembly()
        {
            var col = new PluginsCollection();
            Assert.Throws<PluginsException>(() => col.LoadPlugin("badAsm"));
        }

        [Fact]
        public void LoadPluginByBadAsmAndName()
        {
            var col = new PluginsCollection();
            Assert.Throws<PluginsException>(() => col.LoadPlugin("Sosososbad", TEST_PLUGIN1_FULLNAME));
        }

        [Fact]
        public void LoadPluginByAsmAndBadName()
        {
            var col = new PluginsCollection();
            Assert.Throws<TypeLoadException>(() => col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, "badfullname"));
        }

        [Fact]
        public void LoadPluginByAsmAndMissingName()
        {
            var col = new PluginsCollection();
            Assert.Throws<PluginsException>(() => col.LoadPlugin(TEST_PLUGIN1_ASSEMBLY, null));
        }

        [Fact]
        public void LoadPluginByMissingAsmAndName()
        {
            var col = new PluginsCollection();
            Assert.Throws<PluginsException>(() => col.LoadPlugin(null, TEST_PLUGIN1_FULLNAME));
        }
    }
}
