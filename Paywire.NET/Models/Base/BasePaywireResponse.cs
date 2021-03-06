using System.Xml.Serialization;

namespace Paywire.NET.Models.Base;

public class BasePaywireResponse
{
    /// <summary>
    /// Status for the request.	APPROVAL, DECLINED, ERROR, SUCCESS, CAPTURED, CHARGEBACK
    /// </summary>
    [XmlElement("RESULT")]
    public PaywireResult Result { get; set; }

    /// <summary>
    /// Paywire-generated unique merchant identifier.	
    /// </summary>
    public string PWCLIENTID { get; set; }
    /// <summary>
    /// Identifier for this request.	
    /// </summary>
    public string PWINVOICENUMBER { get; set; }

    /// <summary>
    /// Contains the error message.
    /// </summary>
    public string RESTEXT { get; set; }

    public string PWCUSTOMERID { get; set; }

    public string PWCID { get; set; }

    public string PWTOKEN { get; set; }
}