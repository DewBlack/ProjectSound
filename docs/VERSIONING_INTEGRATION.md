# IntegraciÃ³n de versionado â€” ProjectSound

Normativa Ãºnica: [polÃ­tica de studio](https://github.com/pallobbygames/studio/blob/039e84d5e8a71fb897b5ed14365993e56012322c/docs/VERSIONING.md). Este documento registra la integraciÃ³n local.

## Inventario previo (03/11/2026)

| Dato | Observado antes de modificar |
|---|---|
| Repositorio / estado | DewBlack/ProjectSound, personal confirmado, no archivado |
| Motor / producto | Unity 2018.2.14f1; productName `ProjectSound` conservado |
| VersiÃ³n fuente | `0.1`, `ProjectSettings/ProjectSettings.asset` â†’ `bundleVersion` |
| Publicaciones | GitHub: 0 tags / 0 releases; historial de tiendas no disponible, no se deduce versiÃ³n publicada |
| Identificadores | Standalone=com.Company.ProductName; sin Android explÃ­cito; se conservan incluso los heredados/de ejemplo |
| Android | `AndroidBundleVersionCode: 1`, observado en PlayerSettings; no demuestra mÃ¡ximo de Play |
| Consumidores | Unity PlayerSettings â†’ metadatos del player y `Application.version`; no CI propio previo ni scripts de release/versionado observados |
| Ajustes antes | ExistÃ­an, sin etiqueta automÃ¡tica. |
| Plataforma observada | Escenas Unity de escritorio; Android solo tiene el cÃ³digo predeterminado. No se aÃ±aden destinos ni pipelines de tienda. |
| Propuesta | `0.1.0`; normaliza el PATCH implÃ­cito a cero, sin anunciar un lanzamiento nuevo ni cambiar su secuencia. |

## Resultado implementado

Fuente Ãºnica editable: `bundleVersion` en PlayerSettings. Unity deriva `Application.version`
del artefacto; `ProductBuildInfo` lo consulta sin red. No existe otro nÃºmero manual en la interfaz.
MenÃº existente â†’ Opciones â†’ Settings, Assets/Prefabs/MenuPrincipal.prefab. La versiÃ³n aparece en el pie de la pantalla principal de Ajustes, con texto espaÃ±ol/inglÃ©s
y sin bloquear raycasts. El sufijo se conserva completo.

La identidad del producto, cÃ³digos conocidos, firma, dependencias y versiÃ³n del motor no cambian.
No hay asignaciÃ³n Android autorizada hasta conciliar el applicationId real y TODOS los canales
de Play. El contador comÃºn funciona localmente, pero su bootstrap de publicaciÃ³n permanece
pendiente de ese inventario externo; nunca se presupone que 1 sea una cota segura.

## VerificaciÃ³n y ejecuciÃ³n pendiente

Los checks de Python validan SemVer, fuente, contrato UI y sincronizaciÃ³n idempotente.
La CI es de lectura: no construye, sube, etiqueta ni publica. No hay workflows previos de subida.
El Unity exacto `2018.2.14f1` no estÃ¡ instalado en este equipo. La compilaciÃ³n completa, inspecciÃ³n
del artefacto de ESTE juego, captura visual y QA en telÃ©fonos quedan pendientes de instalarlo;
una compilaciÃ³n aislada contra otro Unity no certifica esta versiÃ³n del proyecto.

Con el editor exacto instalado, ejecutar desde este checkout:

```powershell
& '<UNITY_EXACTO>/Editor/Unity.exe' -batchmode -quit -projectPath (Get-Location).Path -executeMethod VersioningBuildChecks.Validate -logFile versioning-editor.log
& '<UNITY_EXACTO>/Editor/Unity.exe' -batchmode -quit -projectPath (Get-Location).Path -executeMethod VersioningBuildChecks.BuildSmoke -logFile versioning-build.log
$version = (python scripts/versioning/stack_version.py show --json | ConvertFrom-Json).version
& ./builds/versioning-smoke/player.exe --version-smoke-output "$PWD/installed-version.json" --expected-version $version --version-smoke-screenshot "$PWD/settings-version.png"
# installed-version.json se escribe desde el player real después de comprobar el pie de Ajustes.
# No confundir build-version.json (procedencia de build) con esta evidencia del player instalado.
```

Abrir la build, entrar en Ajustes, comparar el pie con `show` y metadatos. Probar 1280Ã—720,
1920Ã—1080 y ventana estrecha/vertical si se soporta; ES/EN, teclado/ratÃ³n y retorno al menÃº.
Preparar otra versiÃ³n con sufijo en una rama temporal y repetir sin editar UI.
En Android, verificar ademÃ¡s versionName/versionCode reales con el inspector de APK antes de publicar.

## ReversiÃ³n

Revertir el commit de migraciÃ³n revierte herramientas, menÃº/etiqueta y normalizaciÃ³n; no afecta
datos de juego ni historial publicado. Nunca reciclar reservas Android ni reescribir tags/releases.
La etiqueta consulta solo metadatos locales: no depende del documento central en ejecuciÃ³n.

## Herramientas fijadas y comandos locales

El toolset común se fija en `scripts/versioning.lock.json`; su updater verifica cada hash antes
de reemplazar archivos. SemVer 3.0.4 y contador se reutilizan sin modificación. El adaptador Unity
tiene su propia procedencia inmutable en `scripts/stack-versioning.lock.json`; para actualizarlo
revisar el commit de studio, ejecutar el actualizador con SHA y checksum explícitos y revisar el diff.
No se descarga nada al compilar ni ejecutar el juego.

```powershell
python scripts/versioning/stack_version.py show --json
python scripts/versioning/stack_version.py check
python scripts/versioning/stack_version.py sync --dry-run
python scripts/versioning/stack_version.py sync
python scripts/versioning/stack_version.py propose patch
python scripts/versioning/stack_version.py propose minor --phase beta
python scripts/versioning/stack_version.py prepare 0.2.0-beta.1 --dry-run
# Sustituir el ejemplo por el siguiente objetivo propio; prepare rechaza retrocesos.
python scripts/versioning/stack_version.py changelog --output CHANGELOG.generated.md --dry-run
python -m unittest discover -s scripts/versioning/tests
```

`prepare <objetivo>` aplica la propuesta tras revisarla; `sync` es idempotente. Una estable se
prepara quitando el sufijo del objetivo de su candidata y recompilando, nunca renombrando bytes.
`propose major` solo propone: aplicar MAJOR exige `--allow-major` y una justificación de producto.
Los comandos de inspección/dry-run no reservan códigos. Android se valida con `check --release
--platform android`, que exige una reserva del ledger compartido y datos conciliados de Play.
El editor aplica ese gate antes de una build Android de publicación (sin Development). Las builds locales de desarrollo siguen disponibles. Las builds Windows solo usan `check`.

Después de producir un APK real, usar `artifact-inspect <APK> --aapt2 <ruta>` y
`manifest <APK> --bind-reservation --output <recibo>`; conservar producto, versión, código,
commit, plataforma y checksum. `promotion-verify <APK> <recibo>` comprueba mismos bytes.
La UI no muestra un build ID inventado. No se han producido ni publicado APKs en esta migración.

Actualizar tras revisar un cambio común: `python scripts/versioning/update_stack_tools.py --revision
<SHA-40-del-commit-revisado> --manifest-sha256 <SHA-256-de-STACK_TOOLSET.json> --dry-run` y repetir
sin dry-run tras revisar el resultado. El pin inicial es studio `da742bef274d0c14c2bf16af6bdb50ffa68bc602`.

API oficial: [Application.version (2018.2)](https://docs.unity3d.com/2018.2/Documentation/ScriptReference/Application-version.html),
[PlayerSettings.bundleVersion](https://docs.unity.com/en-us/engine/6000.0/script-reference/unityeditor/playersettings/bundleversion).

## Resultados registrados

- 5 pruebas de integración propias del proyecto: PASS; incluyen cambio temporal de versión,
  dry-run, idempotencia, conservación de ID/código, hashes fijados y enlace real del menú.
- 30 pruebas del toolset común: PASS en una ejecución compartida (35 con la integración de Freefall).
  Incluyen precedencia SemVer, ledger concurrente, reintentos y comprobaciones de artefacto.
- C# nuevo de runtime y editor: compilación aislada PASS con referencias de Unity 2022.3.25f1;
  no es una compilación del juego ni certificación de sus dependencias antiguas.
- Build del juego, captura de Ajustes y HONOR 200 Pro / TECNO KG5j: no ejecutados,
  falta el editor exacto de la tabla. Se conservan todos los números de códigos observados.
