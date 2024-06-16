pipeline {
	agent any
	stages {
		stage('Setup') {
			steps {
				echo 'Starting build...'
			}
		}
		stage('Unit testing') {
			steps {
				echo 'Testing...'
			}			
		}
		stage('Deploy API testing environment') {
			steps {
				sh "docker compose up"

				// Build docker image
				//sh "docker build --no-cache -f "TRS Backend/Dockerfile" -t trsbackend ."
				
				// Kill previous container
				//catchError(buildResult: 'SUCCESS', stageResult: 'SUCCESS') {
				//	sh "docker kill \$(docker ps --format '{{.ID}} {{.Ports}}' | grep '0.0.0.0:3000->' | cut -d ' ' -f1)"	
				//}
				// Run the image with removal of container after completion, 
				// expose host port 80 -> container port 3000, detached (execution can continue)
				//sh "docker run --rm -p 3000:3000 -d trsbackend"
			}
		}
		stage('API Testing') {
			steps {
				sh "newman run api-tests.json --reporters cli,junit --reporter-junit-export 'api-testing-junit-report.xml'"
			}
		}
		stage('Stop API Testing environment and Clean') {
			steps {
				sh "docker compose down"

				// Purge all unused images from docker
				sh "docker image prune -a -f"
			}
		}
	}
}
