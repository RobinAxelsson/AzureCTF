#!/usr/bin/env bash
echo -----------Starting Attempts------------
curl "http://localhost:7071/api/HttpLinks?name=bob&answer=this,environment,are,big"
echo
curl "http://localhost:7071/api/HttpLinks?name=albin&answer=who,are,this,cool"
echo
curl "http://localhost:7071/api/HttpLinks?name=Steffie&answer=global,environment,variables,are,cool"
echo
curl "http://localhost:7071/api/HttpLinks?name=Alex&answer=test,my,stupid,loaf"
echo
echo ------------------Admin----------------------
curl -d "" -X POST "http://localhost:7071/api/HttpAdmin"
echo
curl -d "" -X POST "http://localhost:7071/api/HttpAdmin?cmd=delete"
