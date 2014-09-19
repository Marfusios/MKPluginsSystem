MKPluginsSystem
===============

Simple example of plugins system in the .NET world. 
Every plugin is loaded in separate AppDomain. So you are able to modify plugins at application runtime (very useful on server side).

How to use:
- In your project add a reference to PluginsCore and PluginsHost 
- Create your plugins which inherit from PluginBase, put them into separate assembly and set build output path to directory "Plugins" 
- Use PluginCollection to load plugins by name 
- Create parental class that implement interface IParentPlugin (see class FakeHost in unit tests assembly) 
- Call method Init on every loaded plugin (constructor takes IParentPlugin) 

Check the unit tests for more information.
