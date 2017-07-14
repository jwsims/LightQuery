$testProjects = "LightQuery.Tests", "LightQuery.Tests.Integration"
$testFrameworks = "net461", "netcoreapp1.1"
$testRuns = 1;

& dotnet restore

foreach ($testProject in $testProjects){
    foreach ($testFramework in $testFrameworks){
    & cd "$PSScriptRoot\test\$testProject"
        & dotnet.exe xunit -f $testFramework `
            -xml "$PSScriptRoot\results_$testRuns.testresults"   
			
        $testRuns++
   }
}

& cd $PSScriptRoot