using Import;
using OaiPmhNet.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace OaiPmhNet.Models
{
    public class CVN
    {
        private cvnPdf2CvnRootBeanResponse CVNResponse { get; set; }


        public CVN(Import.cvnPdf2CvnRootBeanResponse pCvn)
        {
            CVNResponse = pCvn;
        }

        private List<Import.CvnBean> GetBeans(Import.cvnPdf2CvnRootBeanResponse pCvn, string pN1, string pN2)
        {
            List<Import.CvnBean> beansreturn = new List<Import.CvnBean>();

            foreach (Import.CvnItemBean itemBean in pCvn.@return.cvnRootBean.Where(x => x.Code == pN1))
            {
                List<Import.CvnBean> beans = new List<Import.CvnBean>();
                if (itemBean.CvnAuthorBean != null)
                {
                    beans.AddRange(itemBean.CvnAuthorBean);
                }
                if (itemBean.CvnBoolean != null)
                {
                    beans.AddRange(itemBean.CvnBoolean);
                }
                if (itemBean.CvnCodeGroup != null)
                {
                    beans.AddRange(itemBean.CvnCodeGroup);
                }
                if (itemBean.CvnDateDayMonthYear != null)
                {
                    beans.AddRange(itemBean.CvnDateDayMonthYear);
                }
                if (itemBean.CvnDateMonthYear != null)
                {
                    beans.AddRange(itemBean.CvnDateMonthYear);
                }
                if (itemBean.CvnDateYear != null)
                {
                    beans.AddRange(itemBean.CvnDateYear);
                }
                if (itemBean.CvnDouble != null)
                {
                    beans.AddRange(itemBean.CvnDouble);
                }
                if (itemBean.CvnDuration != null)
                {
                    beans.AddRange(itemBean.CvnDuration);
                }
                if (itemBean.CvnEntityBean != null)
                {
                    beans.AddRange(itemBean.CvnEntityBean);
                }
                if (itemBean.CvnExternalPKBean != null)
                {
                    beans.AddRange(itemBean.CvnExternalPKBean);
                }
                if (itemBean.CvnFamilyNameBean != null)
                {
                    beans.AddRange(itemBean.CvnFamilyNameBean);
                }
                if (itemBean.CvnPageBean != null)
                {
                    beans.AddRange(itemBean.CvnPageBean);
                }
                if (itemBean.CvnPhoneBean != null)
                {
                    beans.AddRange(itemBean.CvnPhoneBean);
                }
                if (itemBean.CvnPhotoBean != null)
                {
                    beans.AddRange(itemBean.CvnPhotoBean);
                }
                if (itemBean.CvnRichText != null)
                {
                    beans.AddRange(itemBean.CvnRichText);
                }
                if (itemBean.CvnString != null)
                {
                    beans.AddRange(itemBean.CvnString);
                }
                if (itemBean.CvnTitleBean != null)
                {
                    beans.AddRange(itemBean.CvnTitleBean);
                }
                if (itemBean.CvnVolumeBean != null)
                {
                    beans.AddRange(itemBean.CvnVolumeBean);
                }

                foreach (Import.CvnBean itemBeanInt in beans.Where(x => x.Code == pN2))
                {
                    beansreturn.Add(itemBeanInt);
                    //switch (itemBeanInt)
                    //{
                    //    case Import.CvnAuthorBean x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnBoolean x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnCodeGroup x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnDateDayMonthYear x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnDateMonthYear x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnDateYear x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnDouble x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnDuration x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnEntityBean x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnExternalPKBean x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnFamilyNameBean x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnPageBean x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnPhoneBean x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnPhotoBean x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnRichText x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnString x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnTitleBean x:
                    //        throw new Exception("xx");
                    //        break;
                    //    case Import.CvnVolumeBean x:
                    //        throw new Exception("xx");
                    //        break;
                    //}
                }
            }
            return beansreturn;

        }

        public Record ToRecord(IOaiConfiguration oConfiguration)
        {
            
            //XNamespace itemNamespace = configurationMapper.namespaces.First(x=>x.key== "xmlns:"+  configurationMapper.item.Split(":")[0]).value;
            //var xml = new XElement(itemNamespace + configurationMapper.item.Split(":")[1]);

            //foreach (Namespace name_space in configurationMapper.namespaces)
            //{

            //    dd.Add(new XAttribute(name_space.key +":"+ name_space.value, name_space.value));
            //}

            var w = new XElement(OaiNamespaces.OaiDcNamespace + "dc",
                    new XAttribute(XNamespace.Xmlns + "oai_dc", OaiNamespaces.OaiDcNamespace),
                    new XAttribute(XNamespace.Xmlns + "dc", OaiNamespaces.DcNamespace),
                    new XAttribute(XNamespace.Xmlns + "xsi", OaiNamespaces.XsiNamespace),
                    new XAttribute(OaiNamespaces.XsiNamespace + "schemaLocation", OaiNamespaces.OaiDcSchemaLocation));

            //var w = new XElement(OaiNamespaces.OaiDcNamespace + "dc",
            //        new XAttribute(XNamespace.Xmlns + "oai_dc", OaiNamespaces.OaiDcNamespace),
            //        new XAttribute(XNamespace.Xmlns + "dc", OaiNamespaces.DcNamespace),
            //        new XAttribute(XNamespace.Xmlns + "xsi", OaiNamespaces.XsiNamespace),
            //        new XAttribute(OaiNamespaces.XsiNamespace + "schemaLocation", OaiNamespaces.OaiDcSchemaLocation),
            //        EncodeList(OaiNamespaces.DcNamespace + "title", metadata.Title),
            //        EncodeList(OaiNamespaces.DcNamespace + "creator", metadata.Creator),
            //        EncodeList(OaiNamespaces.DcNamespace + "subject", metadata.Subject),
            //        EncodeList(OaiNamespaces.DcNamespace + "description", metadata.Description),
            //        EncodeList(OaiNamespaces.DcNamespace + "publisher", metadata.Publisher),
            //        EncodeList(OaiNamespaces.DcNamespace + "contributor", metadata.Contributor),
            //        EncodeList(OaiNamespaces.DcNamespace + "date", metadata.Date, (date) =>
            //        {
            //            return _dateConverter.Encode(_configuration.Granularity, date);
            //        }),
            //        EncodeList(OaiNamespaces.DcNamespace + "type", metadata.Type),
            //        EncodeList(OaiNamespaces.DcNamespace + "format", metadata.Format),
            //        EncodeList(OaiNamespaces.DcNamespace + "identifier", metadata.Identifier),
            //        EncodeList(OaiNamespaces.DcNamespace + "source", metadata.Source),
            //        EncodeList(OaiNamespaces.DcNamespace + "language", metadata.Language),
            //        EncodeList(OaiNamespaces.DcNamespace + "relation", metadata.Relation),
            //        EncodeList(OaiNamespaces.DcNamespace + "coverage", metadata.Coverage),
            //        EncodeList(OaiNamespaces.DcNamespace + "rights", metadata.Rights));



            ////Nombre
            //List<Import.CvnBean> nombre = GetBeans(pCvn, "000.010.000.000", "000.010.000.020");
            //if (nombre != null && nombre.Count > 0 && nombre[0] is Import.CvnString)
            //{
            //    Title = ((Import.CvnString)(nombre[0])).Value;
            //}
            ////Apellidos
            //List<Import.CvnBean> apellidos = GetBeans(pCvn, "000.010.000.000", "000.010.000.010");



            //¿Nombre+apellidos = p2:title?


            DateConverter dateConverter = new DateConverter();
            DublinCoreMetadataConverter dublinCoreMetadataConverter = new DublinCoreMetadataConverter(oConfiguration, dateConverter);

            var x = dublinCoreMetadataConverter.Encode(new DublinCoreMetadata()
            {
                Title = new List<string>() { "aa", "bb" },
                Date = new List<DateTime>() { DateTime.Now },
                Identifier = new List<string> { "aa", "bb" }
            });

            return new Record()
            {
                Header = new RecordHeader()
                {
                    Identifier = "aa",
                    Datestamp = DateTime.Now
                },
                Metadata = new RecordMetadata()
                {
                    Content = dublinCoreMetadataConverter.Encode(new DublinCoreMetadata()
                    {
                        Title = new List<string>() { "aa","bb" },
                        Date = new List<DateTime>() { DateTime.Now },
                        Identifier = new List<string> { "aa", "bb" }
                    })
                }
            };
        }
    }
}
