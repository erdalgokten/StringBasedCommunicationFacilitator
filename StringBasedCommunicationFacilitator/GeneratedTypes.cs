using System;
using System.Text;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Diagnostics;
using System.ServiceModel.Web;
using System.EnterpriseServices;
using StringBasedCommunicationFacilitator.Util;
using StringBasedCommunicationFacilitator.Interfaces;

namespace StringBasedCommunicationFacilitator
{
	#region CustomTypes
	[DataContract]
	public class Order : ICustomType
	{
		public const int TYPE_LENGTH = 50;

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Desc { get; set; }
		[DataMember]
		public decimal? Price { get; set; }
		[DataMember]
		public int Quantity { get; set; }
		[DataMember]
		public decimal? Amount { get; set; }

		public Order()
		{
			// do nothing, just to support new()
		}

		public Order(string str)
		{
			this.FromCustomString(str);
		}

		public override string ToString()
		{
			return this.ToCustomString();
		}

		public int Length
		{
			get { return TYPE_LENGTH; }
		}

		public string ToCustomString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(this.Name.ToCustomString(10, Alignment.LEFT, ' ', "", false));
			sb.Append(this.Desc.ToCustomString(20, Alignment.LEFT, ' ', "", false));
			sb.Append(this.Price.ToCustomString(7, Alignment.RIGHT, '0', "0000.00;-000.00", false));
			sb.Append(this.Quantity.ToCustomString(5, Alignment.RIGHT, '0', "00000", false));
			sb.Append(this.Amount.ToCustomString(8, Alignment.RIGHT, '0', "00000.00;-0000.00", false));

			return sb.ToString();
		}

		public void FromCustomString(string str)
		{
			this.Name = str.Substring(0, 10).ToCSharpType<string>(10, Alignment.LEFT, ' ', "", false);
			this.Desc = str.Substring(10, 20).ToCSharpType<string>(20, Alignment.LEFT, ' ', "", false);
			this.Price = str.Substring(30, 7).ToCSharpType<decimal?>(7, Alignment.RIGHT, '0', "0000.00;-000.00", false);
			this.Quantity = str.Substring(37, 5).ToCSharpType<int>(5, Alignment.RIGHT, '0', "00000", false);
			this.Amount = str.Substring(42, 8).ToCSharpType<decimal?>(8, Alignment.RIGHT, '0', "00000.00;-0000.00", false);
		}
	}
	#endregion

	#region WCF Service Interface
	[ServiceContract]
	public interface IStringBasedCommunicationFacilitatorService
	{
		[OperationContract]
		[WebInvoke(UriTemplate = "GetCustomerOrders", BodyStyle = WebMessageBodyStyle.Bare)]
		[Description("Gets customer orders from the backing system which only lets string based communication")]
		ResponseBagGetCustomerOrders GetCustomerOrders(RequestBagGetCustomerOrders requestBag);
	}
	#endregion

	#region GetCustomerOrders Types
	[DataContract]
	public class RequestBagGetCustomerOrders : IRequestBag
	{
		[DataMember]
		public RequestHeaderGetCustomerOrders RQHeader { get; set; }
		[DataMember]
		public List<RequestBodyGetCustomerOrders> RQBody { get; set; }

		public string ActionName
		{
			get { return "GetCustomerOrders"; }
		}

		public string ActionDesc
		{
			get { return "Gets customer orders from the backing system which only lets string based communication"; }
		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext ctxt)
		{
			// not implemented
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			if (this.RQHeader != null)
			{
				sb.Append(this.RQHeader.ToString());
			}

			if (this.RQBody != null)
			{
				foreach (var bodyItem in this.RQBody)
				{
					sb.Append(bodyItem.ToString());
				}
			}

			return sb.ToString();
		}
	}
	[DataContract]
	public class RequestHeaderGetCustomerOrders
	{
		public const int HEADER_LENGTH = 14;

		[DataMember]
		public DateTime? QueryTime { get; set; }

		public RequestHeaderGetCustomerOrders(string str)
		{
			this.QueryTime = str.Substring(0, 14).ToCSharpType<DateTime?>(14, Alignment.RIGHT, '0', "yyyyMMddhhmmss", false);
		}
		
		[OnDeserialized]
		void OnDeserialized(StreamingContext ctxt)
		{
			if (this.QueryTime == null)
				this.QueryTime = DateTime.Now;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(this.QueryTime.ToCustomString(14, Alignment.RIGHT, '0', "yyyyMMddhhmmss", false));

			return sb.ToString();
		}
	}
	[DataContract]
	public class RequestBodyGetCustomerOrders
	{
		public const int BODY_LENGTH = 10;

		[DataMember]
		public int CustomerId { get; set; }
		[DataMember]
		public bool? OnlyPendingOrders { get; set; }

		public RequestBodyGetCustomerOrders(string str)
		{
			this.CustomerId = str.Substring(0, 9).ToCSharpType<int>(9, Alignment.RIGHT, '0', "000000000", false);
			this.OnlyPendingOrders = str.Substring(9, 1).ToCSharpType<bool?>(1, Alignment.LEFT, ' ', "Y;N", false);
		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext ctxt)
		{
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.Append(this.CustomerId.ToCustomString(9, Alignment.RIGHT, '0', "000000000", false));
			sb.Append(this.OnlyPendingOrders.ToCustomString(1, Alignment.LEFT, ' ', "Y;N", false));

			return sb.ToString();
		}
	}
	[DataContract]
	public class ResponseBagGetCustomerOrders : IResponseBag
	{
		[DataMember]
		public string RequestText { get; set; }
		[DataMember]
		public string ResponseText { get; set; }
		[DataMember]
		public ResponseHeaderGetCustomerOrders RSHeader { get; set; }
		[DataMember]
		public List<ResponseBodyGetCustomerOrders> RSBody { get; set; }
		[DataMember]
		public int ResultCode { get; set; }
		[DataMember]
		public string ResultDesc { get; set; }

		public ResponseBagGetCustomerOrders()
		{ }

		public ResponseBagGetCustomerOrders(string response, string request)
		{
			this.ResponseText = response;
			this.RequestText = request;

			if (ResponseHeaderGetCustomerOrders.HEADER_LENGTH > 0)
			{
				foreach (string headerString in this.ResponseText.SplitBy(ResponseHeaderGetCustomerOrders.HEADER_LENGTH))
				{
					this.RSHeader = new ResponseHeaderGetCustomerOrders(headerString);
					break;
				}
			}

			if (ResponseBodyGetCustomerOrders.BODY_LENGTH > 0)
			{
				if (this.ResponseText.Length > ResponseHeaderGetCustomerOrders.HEADER_LENGTH)
				{
					this.RSBody = new List<ResponseBodyGetCustomerOrders>();
					foreach (string bodyString in this.ResponseText.Substring(ResponseHeaderGetCustomerOrders.HEADER_LENGTH).SplitBy(ResponseBodyGetCustomerOrders.BODY_LENGTH))
					{
						this.RSBody.Add(new ResponseBodyGetCustomerOrders(bodyString));
					}
				}
			}
		}
	}
	[DataContract]
	public class ResponseHeaderGetCustomerOrders
	{
		public const int HEADER_LENGTH = 9;

		[DataMember]
		public int? TotalNumOfOrders { get; set; }

		public ResponseHeaderGetCustomerOrders(string str)
		{
			this.TotalNumOfOrders = str.Substring(0, 9).ToCSharpType<int?>(9, Alignment.RIGHT, '0', "000000000", false);
		}
	}
	[DataContract]
	public class ResponseBodyGetCustomerOrders
	{
		public const int BODY_LENGTH = 50;

		[DataMember]
		public Order _Order { get; set; }

		public ResponseBodyGetCustomerOrders(string str)
		{
			this._Order = str.Substring(0, 50).ToCustomType<Order>(1, Alignment.LEFT, ' ', "", false);
		}
	}
	public partial class StringBasedCommunicationFacilitatorService : IStringBasedCommunicationFacilitatorService
	{
		public ResponseBagGetCustomerOrders GetCustomerOrders(RequestBagGetCustomerOrders requestBag)
		{
			var responseBag = new ResponseBagGetCustomerOrders();
			var errcode = 0;
			var message = string.Empty;
			var stcktrc = string.Empty;
			var chrono = new Stopwatch();
			try
			{
				chrono.Start();
				responseBag = this.GetCustomerOrdersUnsafe(requestBag, chrono);
			}
			catch (FacilitatorException mex)
			{
				responseBag.ResultCode = mex.ResultCode;
				responseBag.ResultDesc = mex.ResultDesc;
				errcode = mex.ResultCode;
				message = mex.ResultDesc + " " + mex.Message;
				stcktrc = mex.StackTrace;
			}
			catch (Exception ex)
			{
				FacilitatorException mex = new FacilitatorException(500);
				responseBag.ResultCode = mex.ResultCode;
				responseBag.ResultDesc = mex.ResultDesc;
				errcode = mex.ResultCode;
				message = ex.Message;
				stcktrc = ex.StackTrace;
			}
			finally
			{
				if (chrono.IsRunning) chrono.Stop();
				if (errcode != 0)
				{
					// Log the exception here
				}
				else
				{
					// Log the success here, with the duration (use chrono.ElapsedMilliseconds)
					FacilitatorException mex = new FacilitatorException(0);
					responseBag.ResultCode = mex.ResultCode;
					responseBag.ResultDesc = mex.ResultDesc;
				}
			}
			return responseBag;
		}
		private ResponseBagGetCustomerOrders GetCustomerOrdersUnsafe(RequestBagGetCustomerOrders requestBag, Stopwatch chrono)
		{
			// Authentication
			// Authorization

			var requestString = requestBag.ToString();
			var responseString = StringBasedCommunicator.SendAndReceive(requestString);
			responseString = responseString.TrimEnd(' ');

			var responseBag = new ResponseBagGetCustomerOrders(responseString, requestString);
			responseBag.RequestText = requestBag.ToString();

			return responseBag;
		}
	}
	#endregion

}