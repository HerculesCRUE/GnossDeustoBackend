#!/bin/bash
git clone https://github.com/HerculesCRUE/GnossDeustoBackend.git
git clone https://github.com/HerculesCRUE/oai-pmh.git
cd GnossDeustoBackend/
git pull
docker build -t apicarga src/Hercules.Asio.Api.Carga/API_CARGA/
docker build -t apidiscover src/Hercules.Asio.Api.Discover/API_DISCOVER/
docker build -t apioaipmh src/Hercules.Asio.CVN2OAI_PMH/OAI_PMH_CVN/
docker build -t apicron src/Hercules.Asio.Cron/CronConfigure/
docker build -t apigesdoc src/Hercules.Asio.DinamicPages/GestorDocumentacion/
docker build -t apiidentity src/Hercules.Asio.IdentityServer/IdentityServerHecules/
docker build -t apiuris src/Hercules.Asio.UrisFactory/UrisAutoGenerator/
docker build -t apiunidata src/Unidata/Api_Unidata/Api_Unidata/
docker build -t xmlrdfconversor src/Hercules.Asio.XML_RDF_Conversor/XML_RDF_Conversor/
docker build -t apicvn src/cvn/
docker build -t apibridge src/fair/bridge/
docker build -t bridgeswagger src/fair/bridge/codegen_server/
cd ..
cd oai-pmh
git pull https://github.com/HerculesCRUE/oai-pmh.git
docker build -t apioaipmhxml OAI_PMH_XML/OAI_PMH_XML/
cd ../dock-back
docker-compose down -v
docker-compose up -d
