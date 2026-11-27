# SOUL DEATH

SOUL DEATH is planned as a single-player third-person mystery action-adventure for Windows, built with Unity 6 LTS and URP.

## Current status

Stage 1 is in progress. Unity Editor `6000.6.11f1` is installed. Unity Hub created a URP-based Get Started project at `My project/`, nested under this workspace, rather than at the requested project root. The root still contains the original `Assets/` scripts and docs and is not yet a Unity project. The nested Editor is running in Safe Mode; its log reports that Windows Application Control blocked Unity's `BeeBuildProgramCommon.Data.dll`. No files have been merged, and no Unity compilation or scene tests have passed.

## Next setup step

Resolve the authorized Windows Application Control block for the installed Unity Editor, close the nested `My project` Editor, then safely integrate only the required Unity-generated project files into this root while preserving existing assets. See [SOUL_DEATH_BUILD_GUIDE.md](./SOUL_DEATH_BUILD_GUIDE.md).

## Development stages

The project is being developed one stage at a time. Do not treat later-stage features as implemented until their code and tests exist. See [SOUL_DEATH_DEVELOPMENT_PLAN.md](./SOUL_DEATH_DEVELOPMENT_PLAN.md).
