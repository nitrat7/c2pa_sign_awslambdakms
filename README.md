
# c2pa aws lambda sign demo with aws kms

## Preperations for setup private key import for AWS KMS

1. convert private key in binary format (.der)
   
``
openssl pkcs8 -topk8 -inform PEM -outform DER -in es256_private.key -out es256_private.der -nocrypt
``

2. Import with wrapping algorithm `RSAES_OAEP_SHA_256` and a RSA key 4096 bit, ref. [^1] 

[^1]: https://docs.aws.amazon.com/kms/latest/developerguide/importing-keys-encrypt-key-material.html


## Short Introduction in running c2patool with AWS KMS

1. using parameter `signer-path`, ref [^3] and [^4]

[^3]: https://github.com/contentauth/c2patool?tab=readme-ov-file#signing-claim-bytes-with-your-own-signer

[^4]: https://github.com/nitrat7/c2pa_sign_awslambdakms/blob/main/lambda_c2pasign/runC2PA.cs#L208)

2. and application that gets claim-bytes per standard-input and returns signed bytestream via standard-output, ref [^5]

[^5]: https://github.com/nitrat7/c2pa_sign_awslambdakms/blob/main/kms_signer/Program.cs#L18
