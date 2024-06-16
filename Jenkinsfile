pipeline {
	agent any
	stages {
		stage('Setup') {
			steps {
				echo 'Starting build...'

				// Install Entity Framework for migrations
				sh "dotnet tool install --global dotnet-ef"
			}
		}
		stage('Unit testing') {
			steps {
			 	echo 'Testing...'
			}			
		}
		stage('Deploy API testing environment') {
			steps {
				// Deploy test database
				sh "docker run --rm -p 3306:3306 -e MYSQL_ROOT_PASSWORD=MySuperSecretPassword123 -d mysql:latest"
				
				// Sleep 10 seconds so the database has time to be set up
				sleep(time:10, unit:"SECONDS")

				// Apply migrations to datebase using Entity Framework
				sh "dotnet-ef database update"

				// Build backend API docker image
				sh """
					docker build --no-cache -f "TRS\\ backend/Dockerfile" -t trsbackend .
				"""

				// Run backend API
				sh "docker run --rm -p 3000:3000 -d trsbackend"
			}
		}
		stage('API Testing') {
			steps {
				sh "newman run api-tests.json --reporters cli,junit --reporter-junit-export 'api-testing-junit-report.xml'"
			}
		}
		stage('Stop API Testing environment and Clean') {
			steps {
				// Kill the two docker containers running the database and the backend API
				catchError(buildResult: 'SUCCESS', stageResult: 'SUCCESS') {
					sh "docker kill \$(docker ps -qf expose=3306)"
					sh "docker kill \$(docker ps -qf expose=3000)"
				}

				// Purge all unused images from docker
				sh "docker image prune -a -f"
			}
		}
	}
}
