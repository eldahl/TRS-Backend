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
				// Build docker image
				sh "docker build --no-cache -f "TRS Backend/Dockerfile" -t trsbackend ."
				
				// Kill previous container
				catchError(buildResult: 'SUCCESS', stageResult: 'SUCCESS') {
					sh "docker kill \$(docker ps --format '{{.ID}} {{.Ports}}' | grep '0.0.0.0:3000->' | cut -d ' ' -f1)"	
				}
				// Run the image with removal of container after completion, 
				// expose host port 80 -> container port 3000, detached (execution can continue)
				sh "docker run --rm -p 3000:3000 -d trsbackend"
			}
		}
		stage('Clean') {
			steps {
				// Purge all unused images from docker
				sh "docker image prune -a -f"
			}
		}
	}
}
