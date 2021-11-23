node('master'){
    stage('Clean Workspace'){
        cleanWs()
    }
    
    stage('Fetch from Git'){
        // Correct branch name, if different from 'main' - it can typically be 'master' instead 
        git branch: '${BRANCH_NAME}', url:'https://github.com/au-swte21-14/MicrowaveHandin3.git'
    }
    
    stage('Clean Build') {
        bat '"C:\\Program Files\\dotnet\\dotnet.exe" clean MicrowaveOven.sln'
    }

    stage('Build') {
        bat '"C:\\Program Files\\dotnet\\dotnet.exe" build MicrowaveOven.sln'
    }

    try {
        stage('Run coverage of unit tests') {
            bat '"C:\\Program Files (x86)\\Jetbrains\\dotCover.2020.2.4\\dotCover.exe" analyze dotCoverCoverageConfigCore.xml'
        }
    }
    finally {
        // Always publish test results
        stage('Publish Test Results') {
            nunit testResultsPattern: 'Microwave.Test.Unit\\TestResults\\TestResults.xml'
        }
    }

    // Only publish coverage if all tests passed
    stage('Publish Coverage Results') {
        publishHTML(
            [allowMissing: false, 
             alwaysLinkToLastBuild: true, 
             keepAll: false, 
             reportDir: '.', 
             reportFiles: 'coverage_report.html', 
             reportName: 'Coverage Report', 
             reportTitles: ''
            ])
    }   
}