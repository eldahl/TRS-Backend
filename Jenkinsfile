pipeline {
	agent any

	environment {
		// Set path to include dotnet tools so we can use dotnet ef
		PATH = "$PATH:$HOME/.dotnet/tools/"
	}
	stages {
		stage('Setup') {
			steps {
				echo 'Starting build...'

				// Kill the two docker containers running the database and the backend API if they are running
				catchError(buildResult: 'SUCCESS', stageResult: 'SUCCESS') {
					sh "docker kill \$(docker ps -q --filter ancestor=mysql:latest)"
					sh "docker kill \$(docker ps -q --filter ancestor=trsbackend)"
				}
				
				echo "Path: ${PATH}"

				// Install Entity Framework for migrations
				sh 'dotnet tool install --global dotnet-ef'
			}
		}
		stage('Unit testing') {
			steps {
			 	echo 'Testing...'
	
				catchError { 
					dir('TRS backend test') {
						sh 'dotnet add package coverlet.collector'
						sh 'dotnet add package coverlet.msbuild'
						sh "dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:ExcludeByFile='**/*Migrations/*.cs'"
					}
				}
			}
			post {
				success {
					recordCoverage(tools: [[parser: 'COBERTURA', pattern: '<fileset><filename name="TRS backend test/coverage.cobertura.xml"/></fileset>']])
				}
			}
		}
		stage('Deploy API testing environment') {
			steps {
				// Deploy test database
				sh """
					docker run --rm -p 3306:3306 -e MYSQL_ROOT_HOST="%" -e MYSQL_ROOT_PASSWORD=MySuperSecretPassword123 -d mysql:latest
				"""
				
				// Sleep 10 seconds so the database has time to be set up
				sleep(time:20, unit:"SECONDS")

				// Apply migrations to datebase using Entity Framework
				dir('TRS backend') {
					sh "dotnet-ef database update --configuration Release -- --environment Release"
				}

				// Build backend API docker image
				sh """
					docker build --no-cache -f "TRS backend/Dockerfile" -t trsbackend .
				"""

				// Run backend API
				sh "docker run --rm --network=host -d trsbackend"
				
				// Sleep 10 seconds so the API has time to initialize
				sleep(time:10, unit:"SECONDS")
			}
		}
		stage('API Testing') {
			steps {
				catchError {
					sh "newman run api-tests.json --reporters cli,junit --reporter-junit-export 'api-testing-junit-report.xml'"
				}
				junit skipPublishingChecks: true, testResults: 'api-testing-junit-report.xml'
			}
		}
		stage('Stop API Testing environment and Clean') {
			steps {
				// Kill the two docker containers running the database and the backend API
				catchError(buildResult: 'SUCCESS', stageResult: 'SUCCESS') {
					sh "docker kill \$(docker ps -q --filter ancestor=mysql:latest)"
					sh "docker kill \$(docker ps -q --filter ancestor=trsbackend)"

					// Remove all exited containers
					sh "docker rm \$(docker ps -a -f status=exited -q)"
				}
			}
		}
	}
}
