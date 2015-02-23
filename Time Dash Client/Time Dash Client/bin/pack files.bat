rmdir /S /Q .\TimeDash

md .\TimeDash
md .\TimeDash\Maps
md .\TimeDash\Res
md .\TimeDash\Shaders

copy .\Debug\Maps\* .\TimeDash\Maps\
copy .\Debug\Res\* .\TimeDash\Res\
copy .\Debug\Shaders\* .\TimeDash\Shaders\

copy .\Debug\openal32.dll .\TimeDash\
copy .\Debug\OpenTK.Compatibility.dll .\TimeDash\
copy .\Debug\OpenTK.dll .\TimeDash\
copy ".\Debug\Time Dash Client.exe" .\TimeDash\
copy ".\Debug\Time Dash Protocol.dll" .\TimeDash\
copy .\Debug\TKTools.dll .\TimeDash\
copy .\Debug\UDPEngine.dll .\TimeDash\
copy .\Debug\icon.ico .\TimeDash\