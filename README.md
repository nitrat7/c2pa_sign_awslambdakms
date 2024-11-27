
# c2pa aws lambda sign demo with aws kms

## Preperations for setup private key import for AWS KMS

1. convert private key in binary format (.der)
   
``
openssl pkcs8 -topk8 -inform PEM -outform DER -in es256_private.key -out es256_private.der -nocrypt
``

2. Import with wrapping algorithm `RSAES_OAEP_SHA_256` and a RSA key 4096 bit, ref. [^1] 

[^1]: https://docs.aws.amazon.com/kms/latest/developerguide/importing-keys-encrypt-key-material.html


## Short Introduction in running c2patool with AWS KMS

![system schema](doc/c2paSign.drawio.png)

1. There's a trigger configured, that once an Object on S3 Bucket has been created in folder "s3BucketPath" (defined by env-variable, default "data"), a call to Lambda function will be initiated. (ref https://github.com/nitrat7/c2pa_sign_awslambdakms/blob/4a185dc5502490e891a8de1c4f493726f3b01be6/lambda_c2pasign/Function.cs#L35)
2. Lambda Function will download Object to local Store 
3. Starting Signing with given manifest-definition (ref https://github.com/nitrat7/c2pa_sign_awslambdakms/blob/4a185dc5502490e891a8de1c4f493726f3b01be6/lambda_c2pasign/runC2PA.cs#L201). 
To be signed claim-bytes will be sent to AWS KMS  - and with stored Config with private Key on AWS KMS (ref https://github.com/nitrat7/c2pa_sign_awslambdakms/blob/main/kms_signer/Program.cs)
Have a look using parameter `signer-path`, (ref https://github.com/nitrat7/c2pa_sign_awslambdakms/blob/main/lambda_c2pasign/runC2PA.cs#L208) and https://github.com/contentauth/c2patool?tab=readme-ov-file#signing-claim-bytes-with-your-own-signer
The kms_signer application that gets claim-bytes per standard-input and returns signed bytestream via standard-output (https://github.com/nitrat7/c2pa_sign_awslambdakms/blob/main/kms_signer/Program.cs#L18)
4. the signed claim bytes will be returned
5. the signed Object will be transferred back to S3-Bucket  in folder "s3BucketPathSigned" (defined by env-variable, default "data_sign")