import sys

idString = sys.argv[1]
print("@Microsoft.KeyVault(SecretUri=" +
      idString.strip().strip("\"id\": \"").strip("\",")+")")
