# Download Docker for Windows
docker version

# list images
docker image list

# pull image from the hub (will need to switch to Windows Containers)
docker pull microsoft/nanoserver

docker pull microsoft/aspnetcore
docker image list

# inspect images
docker image inspect microsoft/nanoserver

# start container
docker container run -it microsoft/nanoserver

# List running containers
docker container ps

# stop running container
docker container stop e997d01dd040

# delete container
docker container rm e997d01dd040

# list all containers 
docker container ls --all

# docker file e.g.
# FROM ubuntu:15.04      -- creates a layer from ubuntu
# COPY . /app            -- adds file from docker clients directory to the container
# RUN make /app          -- builds the app with make
# CMD python /app/app.py -- which command to run in a container

# NOTE: run the docker-compose.yml to build a new aspnet core webapp image

docker image ls --all

# tag the new created image
docker image tag webapp:dev athakker/web:dev

# login
docker login

# push the new image
docker image push athakker/web:dev 


# switch to linux container and install / enable Kubernetes

docker image ls

# some basic Kube commands
kubectl version --short
kubectl get nodes
kubectl get pods

# pull aspnetcore from hub
docker pull microsoft/aspnetcore

# Create Kuberenetes deployment
kubectl run kube-deployment --image=microsoft/aspnetcore --port=80 --replicas=3
kubectl get deployments
kubectl get rs
kubectl get pods


