using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Text;

namespace c2panalyze
{
    public class s3Load
    {

        private static string aws_access_key = "";

        private static string aws_secret = "";

        private static RegionEndpoint aws_region;

        private static string errormessage = "";

        public s3Load(string _aws_access_key, string _aws_secret, string _aws_region)
        {
            errormessage = "";
            aws_access_key = _aws_access_key;
            if ((_aws_secret != "") && (_aws_secret != null))
            {
                try
                {
                    aws_secret = ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(_aws_secret));
                }
                catch
                { }
            }
            else
            {
                aws_secret = "";
            }

            aws_region = returnS3Region(_aws_region);

        }

        public async Task<string> putS3Files(List<string> FilesToUploadFinal, string bucket, string s3BucketPath)
        {
            string returnerrors = "";
            try
            {



                var allTasks = new List<Task>();

                int maxconctasks = 2;
                try
                {
                    maxconctasks = Environment.ProcessorCount / 2;

                    if (maxconctasks < 1)
                    {
                        maxconctasks = 1;
                    }
                }
                catch
                { }


                try
                {
                    AmazonS3Client s3client;
                    ////DOWNLOAD TRANSFERUTILITY
                    if ((aws_access_key == ""))
                    {
                        s3client = new AmazonS3Client(
                      new AmazonS3Config
                      {
                          Timeout = TimeSpan.FromSeconds(100),            // Default value is 100 seconds
                          //ReadWriteTimeout = TimeSpan.FromSeconds(300),   // Default value is 300 seconds
                          MaxErrorRetry = 4,                                               // Default value is 4 retries
                          RegionEndpoint = aws_region
                      });
                    }
                    else
                    {
                        s3client = new AmazonS3Client(aws_access_key, aws_secret,
                      new AmazonS3Config
                      {
                          Timeout = TimeSpan.FromSeconds(100),            // Default value is 100 seconds
                          //ReadWriteTimeout = TimeSpan.FromSeconds(300),   // Default value is 300 seconds
                          MaxErrorRetry = 4,                                               // Default value is 4 retries
                          RegionEndpoint = aws_region
                      });
                    }

                    TransferUtilityConfig transferUtilityConfig = new TransferUtilityConfig();
                    if (FilesToUploadFinal.Count > 100)
                    {
                        transferUtilityConfig.ConcurrentServiceRequests = 100;
                    }
                    else if (FilesToUploadFinal.Count > 50)
                    {
                        transferUtilityConfig.ConcurrentServiceRequests = 50;
                    }
                    else
                    {
                        transferUtilityConfig.ConcurrentServiceRequests = 10;
                    }


                    TransferUtility fileTransferUtility = new TransferUtility(s3client, transferUtilityConfig);

                    var throttler = new SemaphoreSlim(initialCount: maxconctasks);
                    foreach (string currfile in FilesToUploadFinal)
                    {
                        throttler.Wait();
                        allTasks.Add(
                            Task.Run(async () =>
                            {
                                try
                                {
                                    var uploadRequest =
                                        new TransferUtilityUploadRequest
                                        {
                                            BucketName = bucket,
                                            //FIXME name data and data_signed need to be parametrized !!!
                                            
                                            Key = Path.Combine(s3BucketPath.TrimStart('/'), currfile.Replace("\\", "/").Replace("/tmp/data/", "").Replace("/tmp/data_sign/", "")),
                                            FilePath = currfile
                                        };

                                    Console.WriteLine("DEBUG Key upload " + Path.Combine(s3BucketPath.TrimStart('/'), currfile.Replace("\\", "/").Replace("/tmp/data/", "").Replace("/tmp/data_sign/", "")));
                                    await fileTransferUtility.UploadAsync(uploadRequest);


                                    throttler.Release();
                                }
                                catch (System.Exception e)
                                {
                                    errormessage = "gets3files3 AWS S3 error2 occurred.Exception: " + e.Message;
                                    throttler.Release();

                                }
                                finally
                                {
                                    throttler.Release();
                                }
                            }));
                    }
                    Task.WhenAll(allTasks).Wait();


                    fileTransferUtility.Dispose();
                    s3client.Dispose();
                }
                catch (AmazonS3Exception amazonS3Exception)
                {
                    errormessage = "gets3files3 AWS S3 error occurred.Exception: " + amazonS3Exception.ToString();
                    return errormessage;
                }
                catch (Exception e)
                {
                    errormessage = "gets3files3 generic S3 error occurred.Exception: " + e.Message;
                    return errormessage;
                }
            }
            catch (System.Exception e)
            {
                returnerrors = e.Message;
            }

            return returnerrors;

        }

        public static Amazon.RegionEndpoint returnS3Region(string _region)
        {
            Amazon.RegionEndpoint AWSEndpoint;

            switch (_region)
            {
                case "eucentral1":
                    AWSEndpoint = Amazon.RegionEndpoint.EUCentral1;
                    break;
                case "eu-central-1":
                    AWSEndpoint = Amazon.RegionEndpoint.EUCentral1;
                    break;
                case "euwest1":
                    AWSEndpoint = Amazon.RegionEndpoint.EUWest1;
                    break;
                case "eu-west-1":
                    AWSEndpoint = Amazon.RegionEndpoint.EUWest1;
                    break;
                case "euwest2":
                    AWSEndpoint = Amazon.RegionEndpoint.EUWest2;
                    break;
                case "eu-west-2":
                    AWSEndpoint = Amazon.RegionEndpoint.EUWest2;
                    break;
                case "euwest3":
                    AWSEndpoint = Amazon.RegionEndpoint.EUWest3;
                    break;
                case "eu-west-3":
                    AWSEndpoint = Amazon.RegionEndpoint.EUWest3;
                    break;
                case "eunorth1":
                    AWSEndpoint = Amazon.RegionEndpoint.EUNorth1;
                    break;
                case "eu-north-1":
                    AWSEndpoint = Amazon.RegionEndpoint.EUNorth1;
                    break;
                case "useast1":
                    AWSEndpoint = Amazon.RegionEndpoint.USEast1;
                    break;
                case "us-east-1":
                    AWSEndpoint = Amazon.RegionEndpoint.USEast1;
                    break;
                case "useast2":
                    AWSEndpoint = Amazon.RegionEndpoint.USEast2;
                    break;
                case "us-east-2":
                    AWSEndpoint = Amazon.RegionEndpoint.USEast2;
                    break;
                case "uswest1":
                    AWSEndpoint = Amazon.RegionEndpoint.USWest1;
                    break;
                case "us-west-1":
                    AWSEndpoint = Amazon.RegionEndpoint.USWest1;
                    break;
                case "uswest2":
                    AWSEndpoint = Amazon.RegionEndpoint.USWest2;
                    break;
                case "us-west-2":
                    AWSEndpoint = Amazon.RegionEndpoint.USWest2;
                    break;
                case "apnortheast1":
                    AWSEndpoint = Amazon.RegionEndpoint.APNortheast1;
                    break;
                case "ap-northeast-1":
                    AWSEndpoint = Amazon.RegionEndpoint.APNortheast1;
                    break;
                case "apnortheast2":
                    AWSEndpoint = Amazon.RegionEndpoint.APNortheast2;
                    break;
                case "ap-northeast-2":
                    AWSEndpoint = Amazon.RegionEndpoint.APNortheast2;
                    break;
                case "apnortheast3":
                    AWSEndpoint = Amazon.RegionEndpoint.APNortheast3;
                    break;
                case "ap-northeast-3":
                    AWSEndpoint = Amazon.RegionEndpoint.APNortheast3;
                    break;
                case "apsouth1":
                    AWSEndpoint = Amazon.RegionEndpoint.APSouth1;
                    break;
                case "ap-south-1":
                    AWSEndpoint = Amazon.RegionEndpoint.APSouth1;
                    break;
                case "apsoutheast1":
                    AWSEndpoint = Amazon.RegionEndpoint.APSoutheast1;
                    break;
                case "ap-southeast-1":
                    AWSEndpoint = Amazon.RegionEndpoint.APSoutheast1;
                    break;
                case "apsoutheast2":
                    AWSEndpoint = Amazon.RegionEndpoint.APSoutheast2;
                    break;
                case "ap-southeast-2":
                    AWSEndpoint = Amazon.RegionEndpoint.APSoutheast2;
                    break;
                case "apcacentral1":
                    AWSEndpoint = Amazon.RegionEndpoint.CACentral1;
                    break;
                case "ap-cacentral-1":
                    AWSEndpoint = Amazon.RegionEndpoint.CACentral1;
                    break;
                default:
                    AWSEndpoint = Amazon.RegionEndpoint.EUWest1;
                    break;
            }
            return AWSEndpoint;
        }
    }
}
