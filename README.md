# ScriptBaseExtended

ScriptBaseExtended is a .NET library intended to be called from the LoginVSI engine via reflection.

## Features:
- Allow scripts to use external libraries
- Provides Syslog, OpenTelemetry, and Victoria-Metrics Support

## Requirements:
- OS: Windows
- .NET Version: Framework 4.8

## Common Issues:

| Issue | Symptoms | Possible Reason | Possible Solution |
| :--- | :--- | :--- | :--- |
| The ApplicationScript compiles but does not run. | A DLL could not be found. | A Reference DLL is not copied to the output directory. | For example try changing <br> ```<Reference Include="System.DirectoryServices"/>```<br>to<br>```<Reference Include="System.DirectoryServices"><Private>True</Private></Reference>``` |
| | The library loads but its the wrong version. | Dependency conflicts. | These suck. I would recommend ditching the library that caused the conflict, It's usually easier than fixing it. |
| | Method 'methodName' with specified parameters not found. | You either forgot to update the boilerplate or are using an old version of the DLL. | Update the boilerplate or DLL. |
| | Strange library errors in the logs | Some libraries just dont play well with reflection. | Swap out the library or open an issue with the developer. |
| The ApplicationScript fails to compile. | Object reference not set to an instance of an object | Type definition issues | Don't use types from libraries as params or return types of methods. Instead use a tuple for the return and common types for the params. |

## Todo:
- [ ] Dynamically support all tuples

---

# ScriptBaseExtendedTools

ScriptBaseExtendedTools is a small GUI utility that generates a boilerplate script with wrapper methods for the `ScriptBaseExtended` class.

## Features:
- Boilerplate generator for ApplicationScripts
- Method Converter for converting methods created for `ScriptBase` for use in `ScriptBaseExtended`
- AppendLine Converter for adding lines to the boilerplate

## Requirements:
- OS: Windows
- .NET Version: 10
- The `ReferenceAssemblies` folder from ScriptEditor copied to this project's root.

---

## Changelog:
See [CHANGELOG.md](CHANGELOG.md)

## Support Policy

This project is provided as an example and educational reference.

While issues and pull requests are welcome, there is no guarantee that reported issues, feature requests, or pull requests will be reviewed or addressed within any specific timeframe.

This project is provided without any support obligations, service level agreements (SLAs), warranties, or guarantees of any kind.

For legal warranty and liability terms, see [LICENSE](LICENSE) included with this project.

## License
This project is released under the AGPL-3.0 license. See [LICENSE](LICENSE).