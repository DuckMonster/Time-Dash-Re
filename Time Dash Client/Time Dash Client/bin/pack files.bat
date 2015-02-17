rmdir /S /Q .\Debug\TimeDash

md .\Debug\TimeDash
md .\Debug\TimeDash\Maps
md .\Debug\TimeDash\Res
md .\Debug\TimeDash\Shaders

copy .\Debug\Maps\* .\Debug\TimeDash\Maps\
copy .\Debug\Res\* .\Debug\TimeDash\Res\
copy .\Debug\Shaders\* .\Debug\TimeDash\Shaders\

copy .\Debug\openal32.dll .\Debug\TimeDash\
copy .\Debug\OpenTK.Compatibility.dll .\Debug\TimeDash\
copy .\Debug\OpenTK.dll .\Debug\TimeDash\
copy ".\Debug\Time Dash Client.exe" .\Debug\TimeDash\
copy ".\Debug\Time Dash Protocol.dll" .\Debug\TimeDash\
copy .\Debug\TKTools.dll .\Debug\TimeDash\
copy .\Debug\UDPEngine.dll .\Debug\TimeDash\
copy .\Debug\icon.ico .\Debug\TimeDash\