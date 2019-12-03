# How to run on Ubuntu

### 1. Build Dali libraries in Ubuntu
- dali-core
- dali-adaptor
- dali-toolkit
- dali-csharp-binder

And note the location where Dali libs are placed.

It is normally `xxx/dali-env/opt/lib`.


### 2. Build NUI.Components in TizenFX
```bash
# Build NUI.Component
$ cd TizenFX
$ sudo ./build.sh build Tizen.NUI.Components
...
Tizen.NUI.Components -> YourDLLPath/Tizen.NUI.Components.dll

# Copy all dll results to `libs` directory of this project.
$ cd projectRoot
$ mkdir libs
$ cp YourDLLPath/*.dll libs
```

### 3. Build Project

```bash
# In the root directory
$ sudo dotnet build
$ sudo LD_LIBRARY_PATH=YourDaliLibPath dotnet run
```
