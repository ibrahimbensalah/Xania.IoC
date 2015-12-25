# Xania.IoC

[![Build status](https://ci.appveyor.com/api/projects/status/fio0d4h9boo2yc0f?svg=true)](https://ci.appveyor.com/project/ibrahimbensalah/xania-ioc)

[![unstable](http://badges.github.io/stability-badges/dist/unstable.svg)](http://github.com/badges/stability-badges)

Why yet another IoC framework? scalability is the magic word. While most (if not all) other IoC frameworks are focused on resolving types, and there lifetime scope, none has really a scalable type registration. The way 'they' do registration is by flattening all types into one container, and the container owner should have knowlegde of all types which breaks the modularity of the application layers.

Here is an example that demonstrates the problem:

