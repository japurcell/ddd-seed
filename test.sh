#!/usr/bin/env bash

#
# variables
#

RESET="\033[0m"
RED="\033[0;31m"
YELLOW="\033[0;33m"
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
run_tests=''
generate_report=''
run_all=false

#
# functions
#

__usage()
{
    echo "Usage: $(basename "${BASH_SOURCE[0]}") [options]
Options:
    --test|-t                         Run all unit tests
    --report|-r                       Generate coverage report
Description:
    This script runs unit tests and generates code coverage reports.
"
}

_test()
{
    dotnet test \
        test/ddd-seed.UnitTests/ddd-seed.UnitTests.csproj \
        /p:CollectCoverage=true \
        /p:CoverletOutputFormat=opencover \
        /p:CoverletOutput=../../coverage/
}

_gen_report()
{
    dotnet tool run reportgenerator -- \
        -reports:"coverage/*.*" \
        -targetdir:"coverage/report"
}

__error() {
    echo -e "${RED}error: $*${RESET}" 1>&2
}

__warn() {
    echo -e "${YELLOW}warning: $*${RESET}"
}

#
# main
#

if [[ $# = 0 ]]; then
    run_all=true
else
    while [[ $# -gt 0 ]]; do
        opt="$(echo "${1/#--/-}" | awk '{print tolower($0)}')"
        case "$opt" in
            -test|-t)
                run_tests=true
                ;;
            -report|-r)
                generate_report=true
                ;;
            *)
                __usage
                exit 1
                ;;
        esac
        shift
    done
fi

if [ "$run_tests" = true ] || [ "$run_all" = true ]; then
    _test
fi

if [ "$generate_report" = true ]  || [ "$run_all" = true ]; then
    _gen_report
fi
