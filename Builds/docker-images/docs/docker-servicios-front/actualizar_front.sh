#!/bin/bash
git clone https://github.com/HerculesCRUE/GnossDeustoBackend.git
cd GnossDeustoBackend/
git pull
docker build -t linkeddataserver src/Hercules.Asio.LinkedDataServer/Linked_Data_Server/
docker build -t apifrontcarga src/Hercules.Asio.Web/ApiCargaWebInterface/
docker build -t benchmark src/Benchmark/triplestore-assessment-interface/
cd ../dock-front
docker-compose down -v
docker-compose up -d
