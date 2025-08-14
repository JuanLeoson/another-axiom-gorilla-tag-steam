using System;
using System.IO;
using System.Text;

namespace LitJson
{
	// Token: 0x02000BC6 RID: 3014
	internal class Lexer
	{
		// Token: 0x1700070E RID: 1806
		// (get) Token: 0x06004901 RID: 18689 RVA: 0x0016398E File Offset: 0x00161B8E
		// (set) Token: 0x06004902 RID: 18690 RVA: 0x00163996 File Offset: 0x00161B96
		public bool AllowComments
		{
			get
			{
				return this.allow_comments;
			}
			set
			{
				this.allow_comments = value;
			}
		}

		// Token: 0x1700070F RID: 1807
		// (get) Token: 0x06004903 RID: 18691 RVA: 0x0016399F File Offset: 0x00161B9F
		// (set) Token: 0x06004904 RID: 18692 RVA: 0x001639A7 File Offset: 0x00161BA7
		public bool AllowSingleQuotedStrings
		{
			get
			{
				return this.allow_single_quoted_strings;
			}
			set
			{
				this.allow_single_quoted_strings = value;
			}
		}

		// Token: 0x17000710 RID: 1808
		// (get) Token: 0x06004905 RID: 18693 RVA: 0x001639B0 File Offset: 0x00161BB0
		public bool EndOfInput
		{
			get
			{
				return this.end_of_input;
			}
		}

		// Token: 0x17000711 RID: 1809
		// (get) Token: 0x06004906 RID: 18694 RVA: 0x001639B8 File Offset: 0x00161BB8
		public int Token
		{
			get
			{
				return this.token;
			}
		}

		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x06004907 RID: 18695 RVA: 0x001639C0 File Offset: 0x00161BC0
		public string StringValue
		{
			get
			{
				return this.string_value;
			}
		}

		// Token: 0x06004908 RID: 18696 RVA: 0x001639C8 File Offset: 0x00161BC8
		static Lexer()
		{
			Lexer.PopulateFsmTables();
		}

		// Token: 0x06004909 RID: 18697 RVA: 0x001639D0 File Offset: 0x00161BD0
		public Lexer(TextReader reader)
		{
			this.allow_comments = true;
			this.allow_single_quoted_strings = true;
			this.input_buffer = 0;
			this.string_buffer = new StringBuilder(128);
			this.state = 1;
			this.end_of_input = false;
			this.reader = reader;
			this.fsm_context = new FsmContext();
			this.fsm_context.L = this;
		}

		// Token: 0x0600490A RID: 18698 RVA: 0x00163A34 File Offset: 0x00161C34
		private static int HexValue(int digit)
		{
			switch (digit)
			{
			case 65:
				break;
			case 66:
				return 11;
			case 67:
				return 12;
			case 68:
				return 13;
			case 69:
				return 14;
			case 70:
				return 15;
			default:
				switch (digit)
				{
				case 97:
					break;
				case 98:
					return 11;
				case 99:
					return 12;
				case 100:
					return 13;
				case 101:
					return 14;
				case 102:
					return 15;
				default:
					return digit - 48;
				}
				break;
			}
			return 10;
		}

		// Token: 0x0600490B RID: 18699 RVA: 0x00163A9C File Offset: 0x00161C9C
		private static void PopulateFsmTables()
		{
			Lexer.fsm_handler_table = new Lexer.StateHandler[]
			{
				new Lexer.StateHandler(Lexer.State1),
				new Lexer.StateHandler(Lexer.State2),
				new Lexer.StateHandler(Lexer.State3),
				new Lexer.StateHandler(Lexer.State4),
				new Lexer.StateHandler(Lexer.State5),
				new Lexer.StateHandler(Lexer.State6),
				new Lexer.StateHandler(Lexer.State7),
				new Lexer.StateHandler(Lexer.State8),
				new Lexer.StateHandler(Lexer.State9),
				new Lexer.StateHandler(Lexer.State10),
				new Lexer.StateHandler(Lexer.State11),
				new Lexer.StateHandler(Lexer.State12),
				new Lexer.StateHandler(Lexer.State13),
				new Lexer.StateHandler(Lexer.State14),
				new Lexer.StateHandler(Lexer.State15),
				new Lexer.StateHandler(Lexer.State16),
				new Lexer.StateHandler(Lexer.State17),
				new Lexer.StateHandler(Lexer.State18),
				new Lexer.StateHandler(Lexer.State19),
				new Lexer.StateHandler(Lexer.State20),
				new Lexer.StateHandler(Lexer.State21),
				new Lexer.StateHandler(Lexer.State22),
				new Lexer.StateHandler(Lexer.State23),
				new Lexer.StateHandler(Lexer.State24),
				new Lexer.StateHandler(Lexer.State25),
				new Lexer.StateHandler(Lexer.State26),
				new Lexer.StateHandler(Lexer.State27),
				new Lexer.StateHandler(Lexer.State28)
			};
			Lexer.fsm_return_table = new int[]
			{
				65542,
				0,
				65537,
				65537,
				0,
				65537,
				0,
				65537,
				0,
				0,
				65538,
				0,
				0,
				0,
				65539,
				0,
				0,
				65540,
				65541,
				65542,
				0,
				0,
				65541,
				65542,
				0,
				0,
				0,
				0
			};
		}

		// Token: 0x0600490C RID: 18700 RVA: 0x00163C84 File Offset: 0x00161E84
		private static char ProcessEscChar(int esc_char)
		{
			if (esc_char <= 92)
			{
				if (esc_char <= 39)
				{
					if (esc_char != 34 && esc_char != 39)
					{
						return '?';
					}
				}
				else if (esc_char != 47 && esc_char != 92)
				{
					return '?';
				}
				return Convert.ToChar(esc_char);
			}
			if (esc_char <= 102)
			{
				if (esc_char == 98)
				{
					return '\b';
				}
				if (esc_char == 102)
				{
					return '\f';
				}
			}
			else
			{
				if (esc_char == 110)
				{
					return '\n';
				}
				if (esc_char == 114)
				{
					return '\r';
				}
				if (esc_char == 116)
				{
					return '\t';
				}
			}
			return '?';
		}

		// Token: 0x0600490D RID: 18701 RVA: 0x00163CEC File Offset: 0x00161EEC
		private static bool State1(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char != 32 && (ctx.L.input_char < 9 || ctx.L.input_char > 13))
				{
					if (ctx.L.input_char >= 49 && ctx.L.input_char <= 57)
					{
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						ctx.NextState = 3;
						return true;
					}
					int num = ctx.L.input_char;
					if (num <= 91)
					{
						if (num <= 39)
						{
							if (num == 34)
							{
								ctx.NextState = 19;
								ctx.Return = true;
								return true;
							}
							if (num != 39)
							{
								return false;
							}
							if (!ctx.L.allow_single_quoted_strings)
							{
								return false;
							}
							ctx.L.input_char = 34;
							ctx.NextState = 23;
							ctx.Return = true;
							return true;
						}
						else
						{
							switch (num)
							{
							case 44:
								break;
							case 45:
								ctx.L.string_buffer.Append((char)ctx.L.input_char);
								ctx.NextState = 2;
								return true;
							case 46:
								return false;
							case 47:
								if (!ctx.L.allow_comments)
								{
									return false;
								}
								ctx.NextState = 25;
								return true;
							case 48:
								ctx.L.string_buffer.Append((char)ctx.L.input_char);
								ctx.NextState = 4;
								return true;
							default:
								if (num != 58 && num != 91)
								{
									return false;
								}
								break;
							}
						}
					}
					else if (num <= 110)
					{
						if (num != 93)
						{
							if (num == 102)
							{
								ctx.NextState = 12;
								return true;
							}
							if (num != 110)
							{
								return false;
							}
							ctx.NextState = 16;
							return true;
						}
					}
					else
					{
						if (num == 116)
						{
							ctx.NextState = 9;
							return true;
						}
						if (num != 123 && num != 125)
						{
							return false;
						}
					}
					ctx.NextState = 1;
					ctx.Return = true;
					return true;
				}
			}
			return true;
		}

		// Token: 0x0600490E RID: 18702 RVA: 0x00163EE4 File Offset: 0x001620E4
		private static bool State2(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 49 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 3;
				return true;
			}
			if (ctx.L.input_char == 48)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 4;
				return true;
			}
			return false;
		}

		// Token: 0x0600490F RID: 18703 RVA: 0x00163F78 File Offset: 0x00162178
		private static bool State3(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
				}
				else
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					int num = ctx.L.input_char;
					if (num <= 69)
					{
						if (num != 44)
						{
							if (num == 46)
							{
								ctx.L.string_buffer.Append((char)ctx.L.input_char);
								ctx.NextState = 5;
								return true;
							}
							if (num != 69)
							{
								return false;
							}
							goto IL_F4;
						}
					}
					else if (num != 93)
					{
						if (num == 101)
						{
							goto IL_F4;
						}
						if (num != 125)
						{
							return false;
						}
					}
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
					IL_F4:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 7;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06004910 RID: 18704 RVA: 0x001640B4 File Offset: 0x001622B4
		private static bool State4(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			int num = ctx.L.input_char;
			if (num <= 69)
			{
				if (num != 44)
				{
					if (num == 46)
					{
						ctx.L.string_buffer.Append((char)ctx.L.input_char);
						ctx.NextState = 5;
						return true;
					}
					if (num != 69)
					{
						return false;
					}
					goto IL_BB;
				}
			}
			else if (num != 93)
			{
				if (num == 101)
				{
					goto IL_BB;
				}
				if (num != 125)
				{
					return false;
				}
			}
			ctx.L.UngetChar();
			ctx.Return = true;
			ctx.NextState = 1;
			return true;
			IL_BB:
			ctx.L.string_buffer.Append((char)ctx.L.input_char);
			ctx.NextState = 7;
			return true;
		}

		// Token: 0x06004911 RID: 18705 RVA: 0x001641A4 File Offset: 0x001623A4
		private static bool State5(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 6;
				return true;
			}
			return false;
		}

		// Token: 0x06004912 RID: 18706 RVA: 0x00164204 File Offset: 0x00162404
		private static bool State6(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
				}
				else
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					int num = ctx.L.input_char;
					if (num <= 69)
					{
						if (num != 44)
						{
							if (num != 69)
							{
								return false;
							}
							goto IL_C9;
						}
					}
					else if (num != 93)
					{
						if (num == 101)
						{
							goto IL_C9;
						}
						if (num != 125)
						{
							return false;
						}
					}
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 1;
					return true;
					IL_C9:
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
					ctx.NextState = 7;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06004913 RID: 18707 RVA: 0x00164314 File Offset: 0x00162514
		private static bool State7(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 8;
				return true;
			}
			int num = ctx.L.input_char;
			if (num == 43 || num == 45)
			{
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
				ctx.NextState = 8;
				return true;
			}
			return false;
		}

		// Token: 0x06004914 RID: 18708 RVA: 0x001643B0 File Offset: 0x001625B0
		private static bool State8(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char >= 48 && ctx.L.input_char <= 57)
				{
					ctx.L.string_buffer.Append((char)ctx.L.input_char);
				}
				else
				{
					if (ctx.L.input_char == 32 || (ctx.L.input_char >= 9 && ctx.L.input_char <= 13))
					{
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					int num = ctx.L.input_char;
					if (num == 44 || num == 93 || num == 125)
					{
						ctx.L.UngetChar();
						ctx.Return = true;
						ctx.NextState = 1;
						return true;
					}
					return false;
				}
			}
			return true;
		}

		// Token: 0x06004915 RID: 18709 RVA: 0x00164485 File Offset: 0x00162685
		private static bool State9(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 114)
			{
				ctx.NextState = 10;
				return true;
			}
			return false;
		}

		// Token: 0x06004916 RID: 18710 RVA: 0x001644AD File Offset: 0x001626AD
		private static bool State10(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 117)
			{
				ctx.NextState = 11;
				return true;
			}
			return false;
		}

		// Token: 0x06004917 RID: 18711 RVA: 0x001644D5 File Offset: 0x001626D5
		private static bool State11(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 101)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x06004918 RID: 18712 RVA: 0x00164503 File Offset: 0x00162703
		private static bool State12(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 97)
			{
				ctx.NextState = 13;
				return true;
			}
			return false;
		}

		// Token: 0x06004919 RID: 18713 RVA: 0x0016452B File Offset: 0x0016272B
		private static bool State13(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 108)
			{
				ctx.NextState = 14;
				return true;
			}
			return false;
		}

		// Token: 0x0600491A RID: 18714 RVA: 0x00164553 File Offset: 0x00162753
		private static bool State14(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 115)
			{
				ctx.NextState = 15;
				return true;
			}
			return false;
		}

		// Token: 0x0600491B RID: 18715 RVA: 0x001644D5 File Offset: 0x001626D5
		private static bool State15(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 101)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x0600491C RID: 18716 RVA: 0x0016457B File Offset: 0x0016277B
		private static bool State16(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 117)
			{
				ctx.NextState = 17;
				return true;
			}
			return false;
		}

		// Token: 0x0600491D RID: 18717 RVA: 0x001645A3 File Offset: 0x001627A3
		private static bool State17(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 108)
			{
				ctx.NextState = 18;
				return true;
			}
			return false;
		}

		// Token: 0x0600491E RID: 18718 RVA: 0x001645CB File Offset: 0x001627CB
		private static bool State18(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 108)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x0600491F RID: 18719 RVA: 0x001645FC File Offset: 0x001627FC
		private static bool State19(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				int num = ctx.L.input_char;
				if (num == 34)
				{
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 20;
					return true;
				}
				if (num == 92)
				{
					ctx.StateStack = 19;
					ctx.NextState = 21;
					return true;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			return true;
		}

		// Token: 0x06004920 RID: 18720 RVA: 0x0016467C File Offset: 0x0016287C
		private static bool State20(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 34)
			{
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x06004921 RID: 18721 RVA: 0x001646AC File Offset: 0x001628AC
		private static bool State21(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num <= 92)
			{
				if (num <= 39)
				{
					if (num != 34 && num != 39)
					{
						return false;
					}
				}
				else if (num != 47 && num != 92)
				{
					return false;
				}
			}
			else if (num <= 102)
			{
				if (num != 98 && num != 102)
				{
					return false;
				}
			}
			else if (num != 110)
			{
				switch (num)
				{
				case 114:
				case 116:
					break;
				case 115:
					return false;
				case 117:
					ctx.NextState = 22;
					return true;
				default:
					return false;
				}
			}
			ctx.L.string_buffer.Append(Lexer.ProcessEscChar(ctx.L.input_char));
			ctx.NextState = ctx.StateStack;
			return true;
		}

		// Token: 0x06004922 RID: 18722 RVA: 0x00164760 File Offset: 0x00162960
		private static bool State22(FsmContext ctx)
		{
			int num = 0;
			int num2 = 4096;
			ctx.L.unichar = 0;
			while (ctx.L.GetChar())
			{
				if ((ctx.L.input_char < 48 || ctx.L.input_char > 57) && (ctx.L.input_char < 65 || ctx.L.input_char > 70) && (ctx.L.input_char < 97 || ctx.L.input_char > 102))
				{
					return false;
				}
				ctx.L.unichar += Lexer.HexValue(ctx.L.input_char) * num2;
				num++;
				num2 /= 16;
				if (num == 4)
				{
					ctx.L.string_buffer.Append(Convert.ToChar(ctx.L.unichar));
					ctx.NextState = ctx.StateStack;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06004923 RID: 18723 RVA: 0x00164854 File Offset: 0x00162A54
		private static bool State23(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				int num = ctx.L.input_char;
				if (num == 39)
				{
					ctx.L.UngetChar();
					ctx.Return = true;
					ctx.NextState = 24;
					return true;
				}
				if (num == 92)
				{
					ctx.StateStack = 23;
					ctx.NextState = 21;
					return true;
				}
				ctx.L.string_buffer.Append((char)ctx.L.input_char);
			}
			return true;
		}

		// Token: 0x06004924 RID: 18724 RVA: 0x001648D4 File Offset: 0x00162AD4
		private static bool State24(FsmContext ctx)
		{
			ctx.L.GetChar();
			if (ctx.L.input_char == 39)
			{
				ctx.L.input_char = 34;
				ctx.Return = true;
				ctx.NextState = 1;
				return true;
			}
			return false;
		}

		// Token: 0x06004925 RID: 18725 RVA: 0x00164910 File Offset: 0x00162B10
		private static bool State25(FsmContext ctx)
		{
			ctx.L.GetChar();
			int num = ctx.L.input_char;
			if (num == 42)
			{
				ctx.NextState = 27;
				return true;
			}
			if (num != 47)
			{
				return false;
			}
			ctx.NextState = 26;
			return true;
		}

		// Token: 0x06004926 RID: 18726 RVA: 0x00164956 File Offset: 0x00162B56
		private static bool State26(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 10)
				{
					ctx.NextState = 1;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06004927 RID: 18727 RVA: 0x00164980 File Offset: 0x00162B80
		private static bool State27(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char == 42)
				{
					ctx.NextState = 28;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06004928 RID: 18728 RVA: 0x001649AC File Offset: 0x00162BAC
		private static bool State28(FsmContext ctx)
		{
			while (ctx.L.GetChar())
			{
				if (ctx.L.input_char != 42)
				{
					if (ctx.L.input_char == 47)
					{
						ctx.NextState = 1;
						return true;
					}
					ctx.NextState = 27;
					return true;
				}
			}
			return true;
		}

		// Token: 0x06004929 RID: 18729 RVA: 0x001649FC File Offset: 0x00162BFC
		private bool GetChar()
		{
			if ((this.input_char = this.NextChar()) != -1)
			{
				return true;
			}
			this.end_of_input = true;
			return false;
		}

		// Token: 0x0600492A RID: 18730 RVA: 0x00164A25 File Offset: 0x00162C25
		private int NextChar()
		{
			if (this.input_buffer != 0)
			{
				int result = this.input_buffer;
				this.input_buffer = 0;
				return result;
			}
			return this.reader.Read();
		}

		// Token: 0x0600492B RID: 18731 RVA: 0x00164A48 File Offset: 0x00162C48
		public bool NextToken()
		{
			this.fsm_context.Return = false;
			while (Lexer.fsm_handler_table[this.state - 1](this.fsm_context))
			{
				if (this.end_of_input)
				{
					return false;
				}
				if (this.fsm_context.Return)
				{
					this.string_value = this.string_buffer.ToString();
					this.string_buffer.Remove(0, this.string_buffer.Length);
					this.token = Lexer.fsm_return_table[this.state - 1];
					if (this.token == 65542)
					{
						this.token = this.input_char;
					}
					this.state = this.fsm_context.NextState;
					return true;
				}
				this.state = this.fsm_context.NextState;
			}
			throw new JsonException(this.input_char);
		}

		// Token: 0x0600492C RID: 18732 RVA: 0x00164B1D File Offset: 0x00162D1D
		private void UngetChar()
		{
			this.input_buffer = this.input_char;
		}

		// Token: 0x04005236 RID: 21046
		private static int[] fsm_return_table;

		// Token: 0x04005237 RID: 21047
		private static Lexer.StateHandler[] fsm_handler_table;

		// Token: 0x04005238 RID: 21048
		private bool allow_comments;

		// Token: 0x04005239 RID: 21049
		private bool allow_single_quoted_strings;

		// Token: 0x0400523A RID: 21050
		private bool end_of_input;

		// Token: 0x0400523B RID: 21051
		private FsmContext fsm_context;

		// Token: 0x0400523C RID: 21052
		private int input_buffer;

		// Token: 0x0400523D RID: 21053
		private int input_char;

		// Token: 0x0400523E RID: 21054
		private TextReader reader;

		// Token: 0x0400523F RID: 21055
		private int state;

		// Token: 0x04005240 RID: 21056
		private StringBuilder string_buffer;

		// Token: 0x04005241 RID: 21057
		private string string_value;

		// Token: 0x04005242 RID: 21058
		private int token;

		// Token: 0x04005243 RID: 21059
		private int unichar;

		// Token: 0x02000BC7 RID: 3015
		// (Invoke) Token: 0x0600492E RID: 18734
		private delegate bool StateHandler(FsmContext ctx);
	}
}
