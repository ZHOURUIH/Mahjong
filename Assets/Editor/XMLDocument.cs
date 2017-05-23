using System;
using System.Text;

public class XMLDocument
{
	// builder
	private StringBuilder builder;

	public XMLDocument()
	{
		builder = new StringBuilder();
	}

	public void startObject(string name)
	{
		builder.AppendFormat("<{0}>", name);
	}

	public void endObject(String name)
	{
		builder.AppendFormat("</{0}>", name);
	}

	public void createElement(string name, string value)
	{
		builder.AppendFormat("<{0}>{1}</{0}>", name, value);
	}

	public override string ToString()
	{
		return builder.ToString();
	}

}