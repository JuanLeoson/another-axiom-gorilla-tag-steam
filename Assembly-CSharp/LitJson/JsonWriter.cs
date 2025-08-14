using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace LitJson
{
	// Token: 0x02000BC4 RID: 3012
	public class JsonWriter
	{
		// Token: 0x1700070A RID: 1802
		// (get) Token: 0x060048DE RID: 18654 RVA: 0x001630A8 File Offset: 0x001612A8
		// (set) Token: 0x060048DF RID: 18655 RVA: 0x001630B0 File Offset: 0x001612B0
		public int IndentValue
		{
			get
			{
				return this.indent_value;
			}
			set
			{
				this.indentation = this.indentation / this.indent_value * value;
				this.indent_value = value;
			}
		}

		// Token: 0x1700070B RID: 1803
		// (get) Token: 0x060048E0 RID: 18656 RVA: 0x001630CE File Offset: 0x001612CE
		// (set) Token: 0x060048E1 RID: 18657 RVA: 0x001630D6 File Offset: 0x001612D6
		public bool PrettyPrint
		{
			get
			{
				return this.pretty_print;
			}
			set
			{
				this.pretty_print = value;
			}
		}

		// Token: 0x1700070C RID: 1804
		// (get) Token: 0x060048E2 RID: 18658 RVA: 0x001630DF File Offset: 0x001612DF
		public TextWriter TextWriter
		{
			get
			{
				return this.writer;
			}
		}

		// Token: 0x1700070D RID: 1805
		// (get) Token: 0x060048E3 RID: 18659 RVA: 0x001630E7 File Offset: 0x001612E7
		// (set) Token: 0x060048E4 RID: 18660 RVA: 0x001630EF File Offset: 0x001612EF
		public bool Validate
		{
			get
			{
				return this.validate;
			}
			set
			{
				this.validate = value;
			}
		}

		// Token: 0x060048E6 RID: 18662 RVA: 0x00163104 File Offset: 0x00161304
		public JsonWriter()
		{
			this.inst_string_builder = new StringBuilder();
			this.writer = new StringWriter(this.inst_string_builder);
			this.Init();
		}

		// Token: 0x060048E7 RID: 18663 RVA: 0x0016312E File Offset: 0x0016132E
		public JsonWriter(StringBuilder sb) : this(new StringWriter(sb))
		{
		}

		// Token: 0x060048E8 RID: 18664 RVA: 0x0016313C File Offset: 0x0016133C
		public JsonWriter(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
			this.Init();
		}

		// Token: 0x060048E9 RID: 18665 RVA: 0x00163160 File Offset: 0x00161360
		private void DoValidation(Condition cond)
		{
			if (!this.context.ExpectingValue)
			{
				this.context.Count++;
			}
			if (!this.validate)
			{
				return;
			}
			if (this.has_reached_end)
			{
				throw new JsonException("A complete JSON symbol has already been written");
			}
			switch (cond)
			{
			case Condition.InArray:
				if (!this.context.InArray)
				{
					throw new JsonException("Can't close an array here");
				}
				break;
			case Condition.InObject:
				if (!this.context.InObject || this.context.ExpectingValue)
				{
					throw new JsonException("Can't close an object here");
				}
				break;
			case Condition.NotAProperty:
				if (this.context.InObject && !this.context.ExpectingValue)
				{
					throw new JsonException("Expected a property");
				}
				break;
			case Condition.Property:
				if (!this.context.InObject || this.context.ExpectingValue)
				{
					throw new JsonException("Can't add a property here");
				}
				break;
			case Condition.Value:
				if (!this.context.InArray && (!this.context.InObject || !this.context.ExpectingValue))
				{
					throw new JsonException("Can't add a value here");
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x060048EA RID: 18666 RVA: 0x00163284 File Offset: 0x00161484
		private void Init()
		{
			this.has_reached_end = false;
			this.hex_seq = new char[4];
			this.indentation = 0;
			this.indent_value = 4;
			this.pretty_print = false;
			this.validate = true;
			this.ctx_stack = new Stack<WriterContext>();
			this.context = new WriterContext();
			this.ctx_stack.Push(this.context);
		}

		// Token: 0x060048EB RID: 18667 RVA: 0x001632E8 File Offset: 0x001614E8
		private static void IntToHex(int n, char[] hex)
		{
			for (int i = 0; i < 4; i++)
			{
				int num = n % 16;
				if (num < 10)
				{
					hex[3 - i] = (char)(48 + num);
				}
				else
				{
					hex[3 - i] = (char)(65 + (num - 10));
				}
				n >>= 4;
			}
		}

		// Token: 0x060048EC RID: 18668 RVA: 0x00163329 File Offset: 0x00161529
		private void Indent()
		{
			if (this.pretty_print)
			{
				this.indentation += this.indent_value;
			}
		}

		// Token: 0x060048ED RID: 18669 RVA: 0x00163348 File Offset: 0x00161548
		private void Put(string str)
		{
			if (this.pretty_print && !this.context.ExpectingValue)
			{
				for (int i = 0; i < this.indentation; i++)
				{
					this.writer.Write(' ');
				}
			}
			this.writer.Write(str);
		}

		// Token: 0x060048EE RID: 18670 RVA: 0x00163394 File Offset: 0x00161594
		private void PutNewline()
		{
			this.PutNewline(true);
		}

		// Token: 0x060048EF RID: 18671 RVA: 0x001633A0 File Offset: 0x001615A0
		private void PutNewline(bool add_comma)
		{
			if (add_comma && !this.context.ExpectingValue && this.context.Count > 1)
			{
				this.writer.Write(',');
			}
			if (this.pretty_print && !this.context.ExpectingValue)
			{
				this.writer.Write('\n');
			}
		}

		// Token: 0x060048F0 RID: 18672 RVA: 0x001633FC File Offset: 0x001615FC
		private void PutString(string str)
		{
			this.Put(string.Empty);
			this.writer.Write('"');
			int length = str.Length;
			int i = 0;
			while (i < length)
			{
				char c = str[i];
				switch (c)
				{
				case '\b':
					this.writer.Write("\\b");
					break;
				case '\t':
					this.writer.Write("\\t");
					break;
				case '\n':
					this.writer.Write("\\n");
					break;
				case '\v':
					goto IL_E4;
				case '\f':
					this.writer.Write("\\f");
					break;
				case '\r':
					this.writer.Write("\\r");
					break;
				default:
					if (c != '"' && c != '\\')
					{
						goto IL_E4;
					}
					this.writer.Write('\\');
					this.writer.Write(str[i]);
					break;
				}
				IL_141:
				i++;
				continue;
				IL_E4:
				if (str[i] >= ' ' && str[i] <= '~')
				{
					this.writer.Write(str[i]);
					goto IL_141;
				}
				JsonWriter.IntToHex((int)str[i], this.hex_seq);
				this.writer.Write("\\u");
				this.writer.Write(this.hex_seq);
				goto IL_141;
			}
			this.writer.Write('"');
		}

		// Token: 0x060048F1 RID: 18673 RVA: 0x00163562 File Offset: 0x00161762
		private void Unindent()
		{
			if (this.pretty_print)
			{
				this.indentation -= this.indent_value;
			}
		}

		// Token: 0x060048F2 RID: 18674 RVA: 0x0016357F File Offset: 0x0016177F
		public override string ToString()
		{
			if (this.inst_string_builder == null)
			{
				return string.Empty;
			}
			return this.inst_string_builder.ToString();
		}

		// Token: 0x060048F3 RID: 18675 RVA: 0x0016359C File Offset: 0x0016179C
		public void Reset()
		{
			this.has_reached_end = false;
			this.ctx_stack.Clear();
			this.context = new WriterContext();
			this.ctx_stack.Push(this.context);
			if (this.inst_string_builder != null)
			{
				this.inst_string_builder.Remove(0, this.inst_string_builder.Length);
			}
		}

		// Token: 0x060048F4 RID: 18676 RVA: 0x001635F7 File Offset: 0x001617F7
		public void Write(bool boolean)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(boolean ? "true" : "false");
			this.context.ExpectingValue = false;
		}

		// Token: 0x060048F5 RID: 18677 RVA: 0x00163627 File Offset: 0x00161827
		public void Write(decimal number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x060048F6 RID: 18678 RVA: 0x00163654 File Offset: 0x00161854
		public void Write(double number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			string text = Convert.ToString(number, JsonWriter.number_format);
			this.Put(text);
			if (text.IndexOf('.') == -1 && text.IndexOf('E') == -1)
			{
				this.writer.Write(".0");
			}
			this.context.ExpectingValue = false;
		}

		// Token: 0x060048F7 RID: 18679 RVA: 0x001636B3 File Offset: 0x001618B3
		public void Write(int number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x060048F8 RID: 18680 RVA: 0x001636DF File Offset: 0x001618DF
		public void Write(long number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x060048F9 RID: 18681 RVA: 0x0016370B File Offset: 0x0016190B
		public void Write(string str)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			if (str == null)
			{
				this.Put("null");
			}
			else
			{
				this.PutString(str);
			}
			this.context.ExpectingValue = false;
		}

		// Token: 0x060048FA RID: 18682 RVA: 0x0016373D File Offset: 0x0016193D
		public void Write(ulong number)
		{
			this.DoValidation(Condition.Value);
			this.PutNewline();
			this.Put(Convert.ToString(number, JsonWriter.number_format));
			this.context.ExpectingValue = false;
		}

		// Token: 0x060048FB RID: 18683 RVA: 0x0016376C File Offset: 0x0016196C
		public void WriteArrayEnd()
		{
			this.DoValidation(Condition.InArray);
			this.PutNewline(false);
			this.ctx_stack.Pop();
			if (this.ctx_stack.Count == 1)
			{
				this.has_reached_end = true;
			}
			else
			{
				this.context = this.ctx_stack.Peek();
				this.context.ExpectingValue = false;
			}
			this.Unindent();
			this.Put("]");
		}

		// Token: 0x060048FC RID: 18684 RVA: 0x001637D8 File Offset: 0x001619D8
		public void WriteArrayStart()
		{
			this.DoValidation(Condition.NotAProperty);
			this.PutNewline();
			this.Put("[");
			this.context = new WriterContext();
			this.context.InArray = true;
			this.ctx_stack.Push(this.context);
			this.Indent();
		}

		// Token: 0x060048FD RID: 18685 RVA: 0x0016382C File Offset: 0x00161A2C
		public void WriteObjectEnd()
		{
			this.DoValidation(Condition.InObject);
			this.PutNewline(false);
			this.ctx_stack.Pop();
			if (this.ctx_stack.Count == 1)
			{
				this.has_reached_end = true;
			}
			else
			{
				this.context = this.ctx_stack.Peek();
				this.context.ExpectingValue = false;
			}
			this.Unindent();
			this.Put("}");
		}

		// Token: 0x060048FE RID: 18686 RVA: 0x00163898 File Offset: 0x00161A98
		public void WriteObjectStart()
		{
			this.DoValidation(Condition.NotAProperty);
			this.PutNewline();
			this.Put("{");
			this.context = new WriterContext();
			this.context.InObject = true;
			this.ctx_stack.Push(this.context);
			this.Indent();
		}

		// Token: 0x060048FF RID: 18687 RVA: 0x001638EC File Offset: 0x00161AEC
		public void WritePropertyName(string property_name)
		{
			this.DoValidation(Condition.Property);
			this.PutNewline();
			this.PutString(property_name);
			if (this.pretty_print)
			{
				if (property_name.Length > this.context.Padding)
				{
					this.context.Padding = property_name.Length;
				}
				for (int i = this.context.Padding - property_name.Length; i >= 0; i--)
				{
					this.writer.Write(' ');
				}
				this.writer.Write(": ");
			}
			else
			{
				this.writer.Write(':');
			}
			this.context.ExpectingValue = true;
		}

		// Token: 0x04005227 RID: 21031
		private static NumberFormatInfo number_format = NumberFormatInfo.InvariantInfo;

		// Token: 0x04005228 RID: 21032
		private WriterContext context;

		// Token: 0x04005229 RID: 21033
		private Stack<WriterContext> ctx_stack;

		// Token: 0x0400522A RID: 21034
		private bool has_reached_end;

		// Token: 0x0400522B RID: 21035
		private char[] hex_seq;

		// Token: 0x0400522C RID: 21036
		private int indentation;

		// Token: 0x0400522D RID: 21037
		private int indent_value;

		// Token: 0x0400522E RID: 21038
		private StringBuilder inst_string_builder;

		// Token: 0x0400522F RID: 21039
		private bool pretty_print;

		// Token: 0x04005230 RID: 21040
		private bool validate;

		// Token: 0x04005231 RID: 21041
		private TextWriter writer;
	}
}
