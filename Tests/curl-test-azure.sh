#!/usr/bin/env bash
echo -----------Starting Attempts------------
curl "https://azurectf-yh4.azurewebsites.net/api/HttpLinks?code=fYd5aVzdRWhNNqc4UKyNwN6nfLd0rzzZf9WBJGV2Sqtxp1mIOO/waw==&name=bob&answer=this,environment,are,big"
echo
curl "https://azurectf-yh4.azurewebsites.net/api/HttpLinks?code=fYd5aVzdRWhNNqc4UKyNwN6nfLd0rzzZf9WBJGV2Sqtxp1mIOO/waw==&name=albin&answer=who,are,this,cool"
echo
curl "https://azurectf-yh4.azurewebsites.net/api/HttpLinks?code=fYd5aVzdRWhNNqc4UKyNwN6nfLd0rzzZf9WBJGV2Sqtxp1mIOO/waw==&name=Steffie&answer=global,environment,variables,are,cool"
echo
curl "https://azurectf-yh4.azurewebsites.net/api/HttpLinks?code=fYd5aVzdRWhNNqc4UKyNwN6nfLd0rzzZf9WBJGV2Sqtxp1mIOO/waw==&name=Alex&answer=test,my,stupid,loaf"
echo
echo ------------------Admin----------------------
curl -d "" -X POST "https://azurectf-yh4.azurewebsites.net/api/HttpAdmin?code=FYKfR3JNceVhMhmVf0pJKC2szaZy/LUzdH8i1CItZfBcakvA7BjyBg=="
echo
curl -d "" -X POST "https://azurectf-yh4.azurewebsites.net/api/HttpAdmin?code=FYKfR3JNceVhMhmVf0pJKC2szaZy/LUzdH8i1CItZfBcakvA7BjyBg==&cmd=delete"
