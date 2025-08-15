#!/bin/bash
set -e

if [ "${TRAVIS_BRANCH}" = "main" ]; then
  if [ "${TRAVIS_PULL_REQUEST}" = "false" ]; then

    echo Starting publish...

    dotnet restore src
    dotnet pack -c Release src

    # todo - work out how to obtain package path from the previous commands
    # as opposed to having to hardcode name/version here
    PACKAGE_PATH=src/bin/Release/Linn.Common.Service.1.0.0.nupkg

    dotnet nuget push "$PACKAGE_PATH" \
      --api-key "$NUGET_API_KEY" \
      --source https://www.nuget.org/api/v2/package \
      --skip-duplicate

    echo ...done publishing
  else
    echo Skipping publish
  fi
else
  echo Skipping publish
fi
