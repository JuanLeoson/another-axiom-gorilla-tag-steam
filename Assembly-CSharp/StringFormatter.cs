using System;
using System.Collections.Generic;
using System.Text;

// Token: 0x0200024E RID: 590
public class StringFormatter
{
	// Token: 0x06000DBF RID: 3519 RVA: 0x0005417F File Offset: 0x0005237F
	public StringFormatter(string[] spans, int[] indices)
	{
		this.spans = spans;
		this.indices = indices;
	}

	// Token: 0x06000DC0 RID: 3520 RVA: 0x00054198 File Offset: 0x00052398
	public string Format(string term1)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			StringFormatter.builder.Append(term1);
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06000DC1 RID: 3521 RVA: 0x00054200 File Offset: 0x00052400
	public string Format(Func<string> term1)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			StringFormatter.builder.Append(term1());
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06000DC2 RID: 3522 RVA: 0x0005426C File Offset: 0x0005246C
	public string Format(string term1, string term2)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			StringFormatter.builder.Append((this.indices[i - 1] == 0) ? term1 : term2);
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06000DC3 RID: 3523 RVA: 0x000542E4 File Offset: 0x000524E4
	public string Format(string term1, string term2, string term3)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			int num = this.indices[i - 1];
			if (num != 0)
			{
				if (num != 1)
				{
					StringFormatter.builder.Append(term3);
				}
				else
				{
					StringFormatter.builder.Append(term2);
				}
			}
			else
			{
				StringFormatter.builder.Append(term1);
			}
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06000DC4 RID: 3524 RVA: 0x0005437C File Offset: 0x0005257C
	public string Format(Func<string> term1, Func<string> term2)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			if (this.indices[i - 1] == 0)
			{
				StringFormatter.builder.Append(term1());
			}
			else
			{
				StringFormatter.builder.Append(term2());
			}
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06000DC5 RID: 3525 RVA: 0x00054408 File Offset: 0x00052608
	public string Format(Func<string> term1, Func<string> term2, Func<string> term3)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			int num = this.indices[i - 1];
			if (num != 0)
			{
				if (num != 1)
				{
					StringFormatter.builder.Append(term3());
				}
				else
				{
					StringFormatter.builder.Append(term2());
				}
			}
			else
			{
				StringFormatter.builder.Append(term1());
			}
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06000DC6 RID: 3526 RVA: 0x000544B0 File Offset: 0x000526B0
	public string Format(Func<string> term1, Func<string> term2, Func<string> term3, Func<string> term4)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			switch (this.indices[i - 1])
			{
			case 0:
				StringFormatter.builder.Append(term1());
				break;
			case 1:
				StringFormatter.builder.Append(term2());
				break;
			case 2:
				StringFormatter.builder.Append(term3());
				break;
			default:
				StringFormatter.builder.Append(term4());
				break;
			}
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06000DC7 RID: 3527 RVA: 0x0005457C File Offset: 0x0005277C
	public string Format(params string[] terms)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			StringFormatter.builder.Append(terms[this.indices[i - 1]]);
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06000DC8 RID: 3528 RVA: 0x000545F0 File Offset: 0x000527F0
	public string Format(params Func<string>[] terms)
	{
		StringFormatter.builder.Clear();
		StringFormatter.builder.Append(this.spans[0]);
		for (int i = 1; i < this.spans.Length; i++)
		{
			StringFormatter.builder.Append(terms[this.indices[i - 1]]());
			StringFormatter.builder.Append(this.spans[i]);
		}
		return StringFormatter.builder.ToString();
	}

	// Token: 0x06000DC9 RID: 3529 RVA: 0x00054668 File Offset: 0x00052868
	public static StringFormatter Parse(string input)
	{
		int num = 0;
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		for (;;)
		{
			int num2 = input.IndexOf('%', num);
			if (num2 == -1)
			{
				break;
			}
			list.Add(input.Substring(num, num2 - num));
			list2.Add((int)(input[num2 + 1] - '0'));
			num = num2 + 2;
		}
		list.Add(input.Substring(num));
		return new StringFormatter(list.ToArray(), list2.ToArray());
	}

	// Token: 0x04001595 RID: 5525
	private static StringBuilder builder = new StringBuilder();

	// Token: 0x04001596 RID: 5526
	private string[] spans;

	// Token: 0x04001597 RID: 5527
	private int[] indices;
}
