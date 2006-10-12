// DataTableTest.cs - NUnit Test Cases for testing the DataTable 
//
// Authors:
//   Franklin Wise (gracenote@earthlink.net)
//   Martin Willemoes Hansen (mwh@sysrq.dk)
//   Hagit Yidov (hagity@mainsoft.com)
// 
// (C) Franklin Wise
// (C) 2003 Martin Willemoes Hansen
// (C) 2005 Mainsoft Corporation (http://www.mainsoft.com)

//
// Copyright (C) 2004 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using NUnit.Framework;
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;

namespace MonoTests.System.Data
{
	[TestFixture]
	public class DataTableTest : Assertion
	{

		[Test]
		public void Ctor()
		{
			DataTable dt = new DataTable();

			AssertEquals("CaseSensitive must be false." ,false,dt.CaseSensitive);
			Assert("Col",dt.Columns != null);
			//Assert(dt.ChildRelations != null);
			Assert("Const", dt.Constraints != null);
			Assert("ds", dt.DataSet == null); 
			Assert("dv", dt.DefaultView != null);
			Assert("de", dt.DisplayExpression == "");
			Assert("ep", dt.ExtendedProperties != null);
			Assert("he", dt.HasErrors == false);
			Assert("lc", dt.Locale != null);
			Assert("mc", dt.MinimumCapacity == 50); //LAMESPEC:
			Assert("ns", dt.Namespace == "");
			//Assert(dt.ParentRelations != null);
			Assert("pf", dt.Prefix == "");
			Assert("pk", dt.PrimaryKey != null);
			Assert("rows", dt.Rows != null);
			Assert("Site", dt.Site == null);
			Assert("tname", dt.TableName == "");
			
		}

		[Test]
                public void Select ()
                {
			DataSet Set = new DataSet ();
			DataTable Mom = new DataTable ("Mom");
			DataTable Child = new DataTable ("Child");			
			Set.Tables.Add (Mom);
			Set.Tables.Add (Child);
			
			DataColumn Col = new DataColumn ("Name");
			DataColumn Col2 = new DataColumn ("ChildName");
			Mom.Columns.Add (Col);
			Mom.Columns.Add (Col2);
			
			DataColumn Col3 = new DataColumn ("Name");
			DataColumn Col4 = new DataColumn ("Age");
			Col4.DataType = Type.GetType ("System.Int16");
			Child.Columns.Add (Col3);
			Child.Columns.Add (Col4);
                	
                	DataRelation Relation = new DataRelation ("Rel", Mom.Columns [1], Child.Columns [0]);
			Set.Relations.Add (Relation);

                	DataRow Row = Mom.NewRow ();
                	Row [0] = "Laura";
                	Row [1] = "Nick";
                	Mom.Rows.Add (Row);
                	
                	Row = Mom.NewRow ();
                	Row [0] = "Laura";
                	Row [1] = "Dick";
                	Mom.Rows.Add (Row);
                	
                	Row = Mom.NewRow ();
                	Row [0] = "Laura";
                	Row [1] = "Mick";
                	Mom.Rows.Add (Row);

                	Row = Mom.NewRow ();
                	Row [0] = "Teresa";
                	Row [1] = "Jack";
                	Mom.Rows.Add (Row);
                	
                	Row = Mom.NewRow ();
                	Row [0] = "Teresa";
                	Row [1] = "Mack";
                	Mom.Rows.Add (Row);

			Row = Mom.NewRow ();
                	Row [0] = "'Jhon O'' Collenal'";
                	Row [1] = "Pack";
                	Mom.Rows.Add (Row);
                	
                	Row = Child.NewRow ();
                	Row [0] = "Nick";
                	Row [1] = 15;
                	Child.Rows.Add (Row);
                	
                	Row = Child.NewRow ();
                	Row [0] = "Dick";
                	Row [1] = 25;
                	Child.Rows.Add (Row);
                	
                	Row = Child.NewRow ();
                	Row [0] = "Mick";
                	Row [1] = 35;
                	Child.Rows.Add (Row);
                	
                	Row = Child.NewRow ();
                	Row [0] = "Jack";
                	Row [1] = 10;
                	Child.Rows.Add (Row);
                	
                	Row = Child.NewRow ();
                	Row [0] = "Mack";
                	Row [1] = 19;
                	Child.Rows.Add (Row);
                
                	Row = Child.NewRow ();
                	Row [0] = "Mack";
                	Row [1] = 99;
                	Child.Rows.Add (Row);

			Row = Child.NewRow ();
                	Row [0] = "Pack";
                	Row [1] = 66;
                	Child.Rows.Add (Row);
                	
                	DataRow [] Rows = Mom.Select ("Name = 'Teresa'");
                	AssertEquals ("test#01", 2, Rows.Length);

			// test with apos escaped
			Rows = Mom.Select ("Name = '''Jhon O'''' Collenal'''");
                	AssertEquals ("test#01.1", 1, Rows.Length);
                	
                	Rows = Mom.Select ("Name = 'Teresa' and ChildName = 'Nick'");
                	AssertEquals ("test#02", 0, Rows.Length);

                	Rows = Mom.Select ("Name = 'Teresa' and ChildName = 'Jack'");
                	AssertEquals ("test#03", 1, Rows.Length);

                	Rows = Mom.Select ("Name = 'Teresa' and ChildName <> 'Jack'");
                	AssertEquals ("test#04", "Mack", Rows [0] [1]);
                	
                	Rows = Mom.Select ("Name = 'Teresa' or ChildName <> 'Jack'");
                	AssertEquals ("test#05", 6, Rows.Length);
			
                	Rows = Child.Select ("age = 20 - 1");
                	AssertEquals ("test#06", 1, Rows.Length);
			
                	Rows = Child.Select ("age <= 20");
                	AssertEquals ("test#07", 3, Rows.Length);
			
                	Rows = Child.Select ("age >= 20");
                	AssertEquals ("test#08", 4, Rows.Length);
			
                	Rows = Child.Select ("age >= 20 and name = 'Mack' or name = 'Nick'");
                	AssertEquals ("test#09", 2, Rows.Length);

                	Rows = Child.Select ("age >= 20 and (name = 'Mack' or name = 'Nick')");
                	AssertEquals ("test#10", 1, Rows.Length);
                	AssertEquals ("test#11", "Mack", Rows [0] [0]);
                	
                	Rows = Child.Select ("not (Name = 'Jack')");
                	AssertEquals ("test#12", 6, Rows.Length);
                }
                
		[Test]
                public void Select2 ()
                {
			DataSet Set = new DataSet ();
			DataTable Child = new DataTable ("Child");

			Set.Tables.Add (Child);
						
			DataColumn Col3 = new DataColumn ("Name");
			DataColumn Col4 = new DataColumn ("Age");
			Col4.DataType = Type.GetType ("System.Int16");
			Child.Columns.Add (Col3);
			Child.Columns.Add (Col4);
                	
                	DataRow Row = Child.NewRow ();
                	Row [0] = "Nick";
                	Row [1] = 15;
                	Child.Rows.Add (Row);
                	
                	Row = Child.NewRow ();
                	Row [0] = "Dick";
                	Row [1] = 25;
                	Child.Rows.Add (Row);
                	
                	Row = Child.NewRow ();
                	Row [0] = "Mick";
                	Row [1] = 35;
                	Child.Rows.Add (Row);
                	
                	Row = Child.NewRow ();
                	Row [0] = "Jack";
                	Row [1] = 10;
                	Child.Rows.Add (Row);
                	
                	Row = Child.NewRow ();
                	Row [0] = "Mack";
                	Row [1] = 19;
                	Child.Rows.Add (Row);
                
                	Row = Child.NewRow ();
                	Row [0] = "Mack";
                	Row [1] = 99;
                	Child.Rows.Add (Row);

			DataRow [] Rows = Child.Select ("age >= 20", "age DESC");
                	AssertEquals ("test#01", 3, Rows.Length);
                	AssertEquals ("test#02", "Mack", Rows [0] [0]);
                	AssertEquals ("test#03", "Mick", Rows [1] [0]);                	
                	AssertEquals ("test#04", "Dick", Rows [2] [0]);                	
                	
                	Rows = Child.Select ("age >= 20", "age asc");
                	AssertEquals ("test#05", 3, Rows.Length);
                	AssertEquals ("test#06", "Dick", Rows [0] [0]);
                	AssertEquals ("test#07", "Mick", Rows [1] [0]);                	
                	AssertEquals ("test#08", "Mack", Rows [2] [0]);                	
                
                	Rows = Child.Select ("age >= 20", "name asc");
                	AssertEquals ("test#09", 3, Rows.Length);
                	AssertEquals ("test#10", "Dick", Rows [0] [0]);
                	AssertEquals ("test#11", "Mack", Rows [1] [0]);                	
                	AssertEquals ("test#12", "Mick", Rows [2] [0]);                	

                	Rows = Child.Select ("age >= 20", "name desc");
                	AssertEquals ("test#09", 3, Rows.Length);
                	AssertEquals ("test#10", "Mick", Rows [0] [0]);
                	AssertEquals ("test#11", "Mack", Rows [1] [0]);                	
                	AssertEquals ("test#12", "Dick", Rows [2] [0]);                	

                }

		[Test]
		public void SelectParsing ()
		{
			DataTable T = new DataTable ("test");
			DataColumn C = new DataColumn ("name");
			T.Columns.Add (C);
			C = new DataColumn ("age");
			C.DataType = typeof (int);
			T.Columns.Add (C);
			C = new DataColumn ("id");
			T.Columns.Add (C);
			
			DataSet Set = new DataSet ("TestSet");
			Set.Tables.Add (T);
			
			DataRow Row = null;
			for (int i = 0; i < 100; i++) {
				Row = T.NewRow ();
				Row [0] = "human" + i;
				Row [1] = i;
				Row [2] = i;
				T.Rows.Add (Row);
			}
			
			Row = T.NewRow ();
			Row [0] = "h*an";
			Row [1] = 1;
			Row [2] = 1;
			T.Rows.Add (Row);

			AssertEquals ("test#01", 12, T.Select ("age<=10").Length);
			
			AssertEquals ("test#02", 12, T.Select ("age\n\t<\n\t=\t\n10").Length);

			try {
				T.Select ("name = 1human ");
				Fail ("test#03");
			} catch (Exception e) {
				
				// missing operand after 'human' operand 
				AssertEquals ("test#04", typeof (SyntaxErrorException), e.GetType ());				
			}
			
			try {			
				T.Select ("name = 1");
				Fail ("test#05");
			} catch (Exception e) {
				
				// Cannot perform '=' operation between string and Int32
				AssertEquals ("test#06", typeof (EvaluateException), e.GetType ());
			}
			
			AssertEquals ("test#07", 1, T.Select ("age = '13'").Length);

		}
		
		[Test]
		public void SelectEscaping () {
			DataTable dt = new DataTable ();
			dt.Columns.Add ("SomeCol");
			dt.Rows.Add (new object [] {"\t"});
			dt.Rows.Add (new object [] {"\\"});
			
			AssertEquals ("test#01", 1, dt.Select (@"SomeCol='\t'").Length);
			AssertEquals ("test#02", 1, dt.Select (@"SomeCol='\\'").Length);
			
			try {
				dt.Select (@"SomeCol='\x'");
				Fail("test#03");
			} catch (SyntaxErrorException) {}
		}

		[Test]
		public void SelectOperators ()
		{
			DataTable T = new DataTable ("test");
			DataColumn C = new DataColumn ("name");
			T.Columns.Add (C);
			C = new DataColumn ("age");
			C.DataType = typeof (int);
			T.Columns.Add (C);
			C = new DataColumn ("id");
			T.Columns.Add (C);
			
			DataSet Set = new DataSet ("TestSet");
			Set.Tables.Add (T);
			
			DataRow Row = null;
			for (int i = 0; i < 100; i++) {
				Row = T.NewRow ();
				Row [0] = "human" + i;
				Row [1] = i;
				Row [2] = i;
				T.Rows.Add (Row);
			}
			
			Row = T.NewRow ();
			Row [0] = "h*an";
			Row [1] = 1;
			Row [2] = 1;
			T.Rows.Add (Row);
			
			AssertEquals ("test#01", 11, T.Select ("age < 10").Length);
			AssertEquals ("test#02", 12, T.Select ("age <= 10").Length);			
			AssertEquals ("test#03", 12, T.Select ("age< =10").Length);			
			AssertEquals ("test#04", 89, T.Select ("age > 10").Length);
			AssertEquals ("test#05", 90, T.Select ("age >= 10").Length);			
			AssertEquals ("test#06", 100, T.Select ("age <> 10").Length);
			AssertEquals ("test#07", 3, T.Select ("name < 'human10'").Length);
			AssertEquals ("test#08", 3, T.Select ("id < '10'").Length);
			// FIXME: Somebody explain how this can be possible.
			// it seems that it is no matter between 10 - 30. The
			// result is allways 25 :-P
			//AssertEquals ("test#09", 25, T.Select ("id < 10").Length);
			
		}

		[Test]
		public void SelectExceptions ()
		{
			DataTable T = new DataTable ("test");
			DataColumn C = new DataColumn ("name");
			T.Columns.Add (C);
			C = new DataColumn ("age");
			C.DataType = typeof (int);
			T.Columns.Add (C);
			C = new DataColumn ("id");
			T.Columns.Add (C);
			
			for (int i = 0; i < 100; i++) {
				DataRow Row = T.NewRow ();
				Row [0] = "human" + i;
				Row [1] = i;
				Row [2] = i;
				T.Rows.Add (Row);
			}
			
			try {
				T.Select ("name = human1");
				Fail ("test#01");
			} catch (Exception e) {
				
				// column name human not found
				AssertEquals ("test#02", typeof (EvaluateException), e.GetType ());
			}
			
			AssertEquals ("test#04", 1, T.Select ("id = '12'").Length);
			AssertEquals ("test#05", 1, T.Select ("id = 12").Length);
			
			try {
				T.Select ("id = 1k3");
				Fail ("test#06");
			} catch (Exception e) {
				
				// no operands after k3 operator
				AssertEquals ("test#07", typeof (SyntaxErrorException), e.GetType ());
			}						
		}
		
		[Test]
		public void SelectStringOperators ()
		{
 			DataTable T = new DataTable ("test");
			DataColumn C = new DataColumn ("name");
			T.Columns.Add (C);
			C = new DataColumn ("age");
			C.DataType = typeof (int);
			T.Columns.Add (C);
			C = new DataColumn ("id");
			T.Columns.Add (C);
			
			DataSet Set = new DataSet ("TestSet");
			Set.Tables.Add (T);
			
			DataRow Row = null;
			for (int i = 0; i < 100; i++) {
				Row = T.NewRow ();
				Row [0] = "human" + i;
				Row [1] = i;
				Row [2] = i;
				T.Rows.Add (Row);
			}
			Row = T.NewRow ();
			Row [0] = "h*an";
			Row [1] = 1;
			Row [2] = 1;
			T.Rows.Add (Row);
					
			AssertEquals ("test#01", 1, T.Select ("name = 'human' + 1").Length);
			
			AssertEquals ("test#02", "human1", T.Select ("name = 'human' + 1") [0] ["name"]);			
			AssertEquals ("test#03", 1, T.Select ("name = 'human' + '1'").Length);
			AssertEquals ("test#04", "human1", T.Select ("name = 'human' + '1'") [0] ["name"]);			
			AssertEquals ("test#05", 1, T.Select ("name = 'human' + 1 + 2").Length);
			AssertEquals ("test#06", "human12", T.Select ("name = 'human' + '1' + '2'") [0] ["name"]);
			
			AssertEquals ("test#07", 1, T.Select ("name = 'huMAn' + 1").Length);
			
			Set.CaseSensitive = true;
			AssertEquals ("test#08", 0, T.Select ("name = 'huMAn' + 1").Length);
			
			T.CaseSensitive = false;
			AssertEquals ("test#09", 1, T.Select ("name = 'huMAn' + 1").Length);
			
			T.CaseSensitive = true;
			AssertEquals ("test#10", 0, T.Select ("name = 'huMAn' + 1").Length);
			
			Set.CaseSensitive = false;
			AssertEquals ("test#11", 0, T.Select ("name = 'huMAn' + 1").Length);
			
			T.CaseSensitive = false;
			AssertEquals ("test#12", 1, T.Select ("name = 'huMAn' + 1").Length);
			
			AssertEquals ("test#13", 0, T.Select ("name = 'human1*'").Length);
			AssertEquals ("test#14", 11, T.Select ("name like 'human1*'").Length);
			AssertEquals ("test#15", 11, T.Select ("name like 'human1%'").Length);
			
			try {
				AssertEquals ("test#16", 11, T.Select ("name like 'h*an1'").Length);
				Fail ("test#16");
			} catch (Exception e) {
				
				// 'h*an1' is invalid
				AssertEquals ("test#17", typeof (EvaluateException), e.GetType ());
			}
			
			try {
				AssertEquals ("test#18", 11, T.Select ("name like 'h%an1'").Length);
				Fail ("test#19");
			} catch (Exception e) {
				
				// 'h%an1' is invalid
				AssertEquals ("test#20", typeof (EvaluateException), e.GetType ());
			}
			
			AssertEquals ("test#21", 0, T.Select ("name like 'h[%]an'").Length);
			AssertEquals ("test#22", 1, T.Select ("name like 'h[*]an'").Length);
			
		}

		[Test]
		public void SelectAggregates ()
		{
			DataTable T = new DataTable ("test");
			DataColumn C = new DataColumn ("name");
			T.Columns.Add (C);
			C = new DataColumn ("age");
			C.DataType = typeof (int);
			T.Columns.Add (C);
			C = new DataColumn ("id");
			T.Columns.Add (C);
			DataRow Row = null;
			
			for (int i = 0; i < 1000; i++) {
				Row = T.NewRow ();
				Row [0] = "human" + i;
				Row [1] = i;
				Row [2] = i;
				T.Rows.Add (Row);
			}
			
			AssertEquals ("test#01", 1000, T.Select ("Sum(age) > 10").Length);
			AssertEquals ("test#02", 1000, T.Select ("avg(age) = 499").Length);
			AssertEquals ("test#03", 1000, T.Select ("min(age) = 0").Length);
			AssertEquals ("test#04", 1000, T.Select ("max(age) = 999").Length);
			AssertEquals ("test#05", 1000, T.Select ("count(age) = 1000").Length);
			AssertEquals ("test#06", 1000, T.Select ("stdev(age) > 287 and stdev(age) < 289").Length);
			AssertEquals ("test#07", 1000, T.Select ("var(age) < 83417 and var(age) > 83416").Length);
		}
		
		[Test]
		public void SelectFunctions ()
		{
			DataTable T = new DataTable ("test");
			DataColumn C = new DataColumn ("name");
			T.Columns.Add (C);
			C = new DataColumn ("age");
			C.DataType = typeof (int);
			T.Columns.Add (C);
			C = new DataColumn ("id");
			T.Columns.Add (C);
			DataRow Row = null;
			
			for (int i = 0; i < 1000; i++) {
				Row = T.NewRow ();
				Row [0] = "human" + i;
				Row [1] = i;
				Row [2] = i;
				T.Rows.Add (Row);
			}
			
			Row = T.NewRow ();
			Row [0] = "human" + "test";
			Row [1] = DBNull.Value;
			Row [2] = DBNull.Value;
			T.Rows.Add (Row);

			//TODO: How to test Convert-function
			AssertEquals ("test#01", 25, T.Select ("age = 5*5") [0]["age"]);			
			AssertEquals ("test#02", 901, T.Select ("len(name) > 7").Length);
			AssertEquals ("test#03", 125, T.Select ("age = 5*5*5 AND len(name)>7") [0]["age"]);
			AssertEquals ("test#04", 1, T.Select ("isnull(id, 'test') = 'test'").Length);			
			AssertEquals ("test#05", 1000, T.Select ("iif(id = '56', 'test', 'false') = 'false'").Length);			
			AssertEquals ("test#06", 1, T.Select ("iif(id = '56', 'test', 'false') = 'test'").Length);
			AssertEquals ("test#07", 9, T.Select ("substring(id, 2, 3) = '23'").Length);
			AssertEquals ("test#08", "123", T.Select ("substring(id, 2, 3) = '23'") [0] ["id"]);
			AssertEquals ("test#09", "423", T.Select ("substring(id, 2, 3) = '23'") [3] ["id"]);
			AssertEquals ("test#10", "923", T.Select ("substring(id, 2, 3) = '23'") [8] ["id"]);
			
		}

		[Test]
		public void SelectRelations ()
		{
                        DataSet Set = new DataSet ();
                        DataTable Mom = new DataTable ("Mom");
                        DataTable Child = new DataTable ("Child");

                        Set.Tables.Add (Mom);
                        Set.Tables.Add (Child);
                        
                        DataColumn Col = new DataColumn ("Name");
                        DataColumn Col2 = new DataColumn ("ChildName");
                        Mom.Columns.Add (Col);
                        Mom.Columns.Add (Col2);
                        
                        DataColumn Col3 = new DataColumn ("Name");
                        DataColumn Col4 = new DataColumn ("Age");
                        Col4.DataType = Type.GetType ("System.Int16");
                        Child.Columns.Add (Col3);
                        Child.Columns.Add (Col4);
                        
                        DataRelation Relation = new DataRelation ("Rel", Mom.Columns [1], Child.Columns [0]);
                        Set.Relations.Add (Relation);

                        DataRow Row = Mom.NewRow ();
                        Row [0] = "Laura";
                        Row [1] = "Nick";
                        Mom.Rows.Add (Row);
                        
                        Row = Mom.NewRow ();
                        Row [0] = "Laura";
                        Row [1] = "Dick";
                        Mom.Rows.Add (Row);
                        
                        Row = Mom.NewRow ();
                        Row [0] = "Laura";
                        Row [1] = "Mick";
                        Mom.Rows.Add (Row);

                        Row = Mom.NewRow ();
                        Row [0] = "Teresa";
                        Row [1] = "Jack";
                        Mom.Rows.Add (Row);
                        
                        Row = Mom.NewRow ();
                        Row [0] = "Teresa";
                        Row [1] = "Mack";
                        Mom.Rows.Add (Row);
                        
                        Row = Child.NewRow ();
                        Row [0] = "Nick";
                        Row [1] = 15;
                        Child.Rows.Add (Row);
                        
                        Row = Child.NewRow ();
                        Row [0] = "Dick";
                        Row [1] = 25;
                        Child.Rows.Add (Row);
                        
                        Row = Child.NewRow ();
                        Row [0] = "Mick";
                        Row [1] = 35;
                        Child.Rows.Add (Row);
                        
                        Row = Child.NewRow ();
                        Row [0] = "Jack";
                        Row [1] = 10;
                        Child.Rows.Add (Row);
                        
                        Row = Child.NewRow ();
                        Row [0] = "Mack";
                        Row [1] = 19;
                        Child.Rows.Add (Row);
                
                        Row = Child.NewRow ();
                        Row [0] = "Mack";
                        Row [1] = 99;
                        Child.Rows.Add (Row);
			
			DataRow [] Rows = Child.Select ("name = Parent.Childname");
			AssertEquals ("test#01", 6, Rows.Length);
			Rows = Child.Select ("Parent.childname = 'Jack'");
			AssertEquals ("test#02", 1, Rows.Length);
			
			/*
			try {
				// FIXME: LAMESPEC: Why the exception is thrown why... why... 
				Mom.Select ("Child.Name = 'Jack'");
				Fail ("test#03");
			} catch (Exception e) {
				AssertEquals ("test#04", typeof (SyntaxErrorException), e.GetType ());
				AssertEquals ("test#05", "Cannot interpret token 'Child' at position 1.", e.Message);
			}
			*/
			
			Rows = Child.Select ("Parent.name = 'Laura'");
			AssertEquals ("test#06", 3, Rows.Length);
			
			DataTable Parent2 = new DataTable ("Parent2");
                        Col = new DataColumn ("Name");
                        Col2 = new DataColumn ("ChildName");

                        Parent2.Columns.Add (Col);
                        Parent2.Columns.Add (Col2);
                        Set.Tables.Add (Parent2);
                        
                        Row = Parent2.NewRow ();
                        Row [0] = "Laura";
                        Row [1] = "Nick";
                        Parent2.Rows.Add (Row);
                        
                        Row = Parent2.NewRow ();
                        Row [0] = "Laura";
                        Row [1] = "Dick";
                        Parent2.Rows.Add (Row);
                        
                        Row = Parent2.NewRow ();
                        Row [0] = "Laura";
                        Row [1] = "Mick";
                        Parent2.Rows.Add (Row);

                        Row = Parent2.NewRow ();
                        Row [0] = "Teresa";
                        Row [1] = "Jack";
                        Parent2.Rows.Add (Row);
                        
                        Row = Parent2.NewRow ();
                        Row [0] = "Teresa";
                        Row [1] = "Mack";
                        Parent2.Rows.Add (Row);

                        Relation = new DataRelation ("Rel2", Parent2.Columns [1], Child.Columns [0]);
                        Set.Relations.Add (Relation);
			
			try {
				Rows = Child.Select ("Parent.ChildName = 'Jack'");
				Fail ("test#07");
			} catch (Exception e) {
				AssertEquals ("test#08", typeof (EvaluateException), e.GetType ());
				//AssertEquals ("test#09", "The table [Child] involved in more than one relation. You must explicitly mention a relation name in the expression 'parent.[ChildName]'.", e.Message);
			}
			
			Rows = Child.Select ("Parent(rel).ChildName = 'Jack'");
			AssertEquals ("test#10", 1, Rows.Length);

			Rows = Child.Select ("Parent(Rel2).ChildName = 'Jack'");
			AssertEquals ("test#10", 1, Rows.Length);
			
			try {
			     	Mom.Select ("Parent.name  = 'John'");
			} catch (Exception e) {
				AssertEquals ("test#11", typeof (IndexOutOfRangeException), e.GetType ());
				AssertEquals ("test#12", "Cannot find relation 0.", e.Message);
			}
			
		}

		[Test]
		public void SelectRowState()
		{
			DataTable d = new DataTable();
			d.Columns.Add (new DataColumn ("aaa"));
			DataRow [] rows = d.Select (null, null, DataViewRowState.Deleted);
			AssertEquals(0, rows.Length);
			d.Rows.Add (new object [] {"bbb"});
			d.Rows.Add (new object [] {"bbb"});
			rows = d.Select (null, null, DataViewRowState.Deleted);
			AssertEquals(0, rows.Length);
		}

		[Test]
		public void ToStringTest()
		{
			DataTable dt = new DataTable();
			dt.Columns.Add("Col1",typeof(int));
			
			dt.TableName = "Mytable";
			dt.DisplayExpression = "Col1";
			
			
			string cmpr = dt.TableName + " + " + dt.DisplayExpression;
			AssertEquals(cmpr,dt.ToString());
		}

		[Test]
		public void PrimaryKey ()
		{
			DataTable dt = new DataTable ();
			DataColumn Col = new DataColumn ();
			Col.AllowDBNull = false;
			Col.DataType = typeof (int);
			dt.Columns.Add (Col);
			dt.Columns.Add ();
			dt.Columns.Add ();
			dt.Columns.Add ();
			
			AssertEquals ("test#01", 0, dt.PrimaryKey.Length);
			
			dt.PrimaryKey = new DataColumn [] {dt.Columns [0]};
			AssertEquals ("test#02", 1, dt.PrimaryKey.Length);
			AssertEquals ("test#03", "Column1", dt.PrimaryKey [0].ColumnName);
			
			dt.PrimaryKey = null;
			AssertEquals ("test#04", 0, dt.PrimaryKey.Length);
			
			Col = new DataColumn ("failed");
			
			try {
				dt.PrimaryKey = new DataColumn [] {Col};
				Fail ("test#05");					
			} catch (Exception e) {
				AssertEquals ("test#06", typeof (ArgumentException), e.GetType ());
				AssertEquals ("test#07", "Column must belong to a table.", e.Message);
			}
			
			DataTable dt2 = new DataTable ();
			dt2.Columns.Add ();
			
			try {
				dt.PrimaryKey = new DataColumn [] {dt2.Columns [0]};
				Fail ("test#08");
			} catch (Exception e) {
				AssertEquals ("test#09", typeof (ArgumentException), e.GetType ());
				AssertEquals ("test#10", "PrimaryKey columns do not belong to this table.", e.Message);
			}
			
			
			AssertEquals ("test#11", 0, dt.Constraints.Count);
			
			dt.PrimaryKey = new DataColumn [] {dt.Columns [0], dt.Columns [1]};
			AssertEquals ("test#12", 2, dt.PrimaryKey.Length);
			AssertEquals ("test#13", 1, dt.Constraints.Count);
			AssertEquals ("test#14", true, dt.Constraints [0] is UniqueConstraint);
			AssertEquals ("test#15", "Column1", dt.PrimaryKey [0].ColumnName);
			AssertEquals ("test#16", "Column2", dt.PrimaryKey [1].ColumnName);
			
		}
		
		[Test]
		public void PropertyExceptions ()
		{
			DataSet set = new DataSet ();
			DataTable table = new DataTable ();
			DataTable table1 =  new DataTable ();
			set.Tables.Add (table);
			set.Tables.Add (table1);

			DataColumn col = new DataColumn ();
			col.ColumnName = "Id";
			col.DataType = Type.GetType ("System.Int32");
			table.Columns.Add (col);
			UniqueConstraint uc = new UniqueConstraint ("UK1", table.Columns[0] );
			table.Constraints.Add (uc);
			table.CaseSensitive = false;
                                                                                                                           
			col = new DataColumn ();
			col.ColumnName = "Name";
			col.DataType = Type.GetType ("System.String");
			table.Columns.Add (col);
        	        
			col = new DataColumn ();
			col.ColumnName = "Id";
			col.DataType = Type.GetType ("System.Int32");
			table1.Columns.Add (col);
			col = new DataColumn ();
			col.ColumnName = "Name";
			col.DataType = Type.GetType ("System.String");
			table1.Columns.Add (col);

			DataRelation dr = new DataRelation ("DR", table.Columns[0], table1.Columns[0]);
			set.Relations.Add (dr);

			try {
				table.CaseSensitive = true;
				table1.CaseSensitive = true;
				Fail ("#A01");
			}
			catch (Exception e) {
				if (e.GetType () != typeof (AssertionException))
					AssertEquals ("#A02", "Cannot change CaseSensitive or Locale property. This change would lead to at least one DataRelation or Constraint to have different Locale or CaseSensitive settings between its related tables.",e.Message);
				else
					Console.WriteLine (e);
			}
			try {
				CultureInfo cultureInfo = new CultureInfo ("en-gb");
				table.Locale = cultureInfo;
				table1.Locale = cultureInfo;
				Fail ("#A03");
			}
			catch (Exception e) {
				 if (e.GetType () != typeof (AssertionException))
					AssertEquals ("#A04", "Cannot change CaseSensitive or Locale property. This change would lead to at least one DataRelation or Constraint to have different Locale or CaseSensitive settings between its related tables.",e.Message);
				else
					Console.WriteLine (e);
			}
			try {
				table.Prefix = "Prefix#1";
				Fail ("#A05");
			}
			catch (Exception e){
				if (e.GetType () != typeof (AssertionException))
					AssertEquals ("#A06", "Prefix 'Prefix#1' is not valid, because it contains special characters.",e.Message);
				else
					Console.WriteLine (e);

			}
		}

		[Test]
		public void GetErrors ()
		{
			DataTable table = new DataTable ();

			DataColumn col = new DataColumn ();
			col.ColumnName = "Id";
			col.DataType = Type.GetType ("System.Int32");
			table.Columns.Add (col);
                                                                                                                             
			col = new DataColumn ();
			col.ColumnName = "Name";
			col.DataType = Type.GetType ("System.String");
			table.Columns.Add (col);
			
			DataRow row = table.NewRow ();
			row ["Id"] = 147;
			row ["name"] = "Abc";
			row.RowError = "Error#1";
			table.Rows.Add (row);

			AssertEquals ("#A01", 1, table.GetErrors ().Length);
			AssertEquals ("#A02", "Error#1", (table.GetErrors ())[0].RowError);
		}
		[Test]
		public void CloneCopyTest ()
		{
			DataTable table = new DataTable ();
			table.TableName = "Table#1";
			DataTable table1 = new DataTable ();
			table1.TableName = "Table#2";
                
			table.AcceptChanges ();
        	        
			DataSet set = new DataSet ("Data Set#1");
			set.DataSetName = "Dataset#1";
			set.Tables.Add (table);
			set.Tables.Add (table1);

			DataColumn col = new DataColumn ();
			col.ColumnName = "Id";
			col.DataType = Type.GetType ("System.Int32");
			table.Columns.Add (col);
			UniqueConstraint uc = new UniqueConstraint ("UK1", table.Columns[0] );
			table.Constraints.Add (uc);
                
			col = new DataColumn ();
			col.ColumnName = "Id";
			col.DataType = Type.GetType ("System.Int32");
			table1.Columns.Add (col);
                                                                                                                             
			col = new DataColumn ();
			col.ColumnName = "Name";
			col.DataType = Type.GetType ("System.String");
			table.Columns.Add (col);
			
			col = new DataColumn ();
			col.ColumnName = "Name";
			col.DataType = Type.GetType ("System.String");
                	table1.Columns.Add (col);
			DataRow row = table.NewRow ();
			row ["Id"] = 147;
			row ["name"] = "Abc";
			row.RowError = "Error#1";
			table.Rows.Add (row);
                                                                                                                             
			row = table.NewRow ();
			row ["Id"] = 47;
			row ["name"] = "Efg";
			table.Rows.Add (row);
			table.AcceptChanges ();
                                                                                                                             
			table.CaseSensitive = true;
			table1.CaseSensitive = true;
			table.MinimumCapacity = 100;
			table.Prefix = "PrefixNo:1";
			table.Namespace = "Namespace#1";
			table.DisplayExpression = "Id / Name + (Id * Id)";
			DataColumn[] colArray = {table.Columns[0]};
			table.PrimaryKey = colArray;
			table.ExtendedProperties.Add ("TimeStamp", DateTime.Now);
#if NET_1_1 // This prevents further tests after .NET 1.1.
#else
			CultureInfo cultureInfo = new CultureInfo ("en-gb");
			table.Locale = cultureInfo;
#endif

			row = table1.NewRow ();
			row ["Name"] = "Abc";
			row ["Id"] = 147;
			table1.Rows.Add (row);

			row = table1.NewRow ();
			row ["Id"] = 47;
			row ["Name"] = "Efg";
			table1.Rows.Add (row);
                
			DataRelation dr = new DataRelation ("DR", table.Columns[0], table1.Columns[0]);
			set.Relations.Add (dr);

			//Testing properties of clone
			DataTable cloneTable = table.Clone ();
			AssertEquals ("#A01",true ,cloneTable.CaseSensitive);
			AssertEquals ("#A02", 0 , cloneTable.ChildRelations.Count);
			AssertEquals ("#A03", 0 , cloneTable.ParentRelations.Count);
			AssertEquals ("#A04", 2,  cloneTable.Columns.Count);
			AssertEquals ("#A05", 1, cloneTable.Constraints.Count);
			AssertEquals ("#A06", "Id / Name + (Id * Id)", cloneTable.DisplayExpression);
			AssertEquals ("#A07", 1 ,cloneTable.ExtendedProperties.Count);
			AssertEquals ("#A08", false ,cloneTable.HasErrors);
#if NET_1_1
#else
			AssertEquals ("#A09", 2057, cloneTable.Locale.LCID);
#endif
			AssertEquals ("#A10", 100, cloneTable.MinimumCapacity);
			AssertEquals ("#A11","Namespace#1", cloneTable.Namespace);
			AssertEquals ("#A12", "PrefixNo:1",cloneTable.Prefix);
			AssertEquals ("#A13", "Id",  cloneTable.PrimaryKey[0].ColumnName);
			AssertEquals ("#A14",0 , cloneTable.Rows.Count );
			AssertEquals ("#A15", "Table#1", cloneTable.TableName);

			//Testing properties of copy
			DataTable copyTable = table.Copy ();
			AssertEquals ("#A16",true ,copyTable.CaseSensitive);
			AssertEquals ("#A17", 0 , copyTable.ChildRelations.Count);
			AssertEquals ("#A18", 0 , copyTable.ParentRelations.Count);
			AssertEquals ("#A19", 2,  copyTable.Columns.Count);
			AssertEquals ("#A20", 1, copyTable.Constraints.Count);
			AssertEquals ("#A21", "Id / Name + (Id * Id)", copyTable.DisplayExpression);
			AssertEquals ("#A22", 1 ,copyTable.ExtendedProperties.Count);
			AssertEquals ("#A23", true ,copyTable.HasErrors);
#if NET_1_1
#else
			AssertEquals ("#A24", 2057, copyTable.Locale.LCID);
#endif
			AssertEquals ("#A25", 100, copyTable.MinimumCapacity);
			AssertEquals ("#A26","Namespace#1", copyTable.Namespace);
			AssertEquals ("#A27", "PrefixNo:1",copyTable.Prefix);
			AssertEquals ("#A28", "Id",  copyTable.PrimaryKey[0].ColumnName);
			AssertEquals ("#A29", 2 , copyTable.Rows.Count );
			AssertEquals ("#A30", "Table#1", copyTable.TableName);
		}

		[Test]
		public void LoadDataException ()
		{
			DataTable table = new DataTable ();
			DataColumn col = new DataColumn ();
			col.ColumnName = "Id";
			col.DataType = Type.GetType ("System.Int32");
			col.DefaultValue = 47;
			table.Columns.Add (col);
			UniqueConstraint uc = new UniqueConstraint ("UK1", table.Columns[0] );
			table.Constraints.Add (uc);
                
			col = new DataColumn ();
			col.ColumnName = "Name";
			col.DataType = Type.GetType ("System.String");
			col.DefaultValue = "Hello";
			table.Columns.Add (col);
                
			table.BeginLoadData();
			object[] row = {147, "Abc"};
			DataRow newRow = table.LoadDataRow (row, true);
                
			object[] row1 = {147, "Efg"};
			DataRow newRow1 = table.LoadDataRow (row1, true);
                                                                                                                             
			object[] row2 = {143, "Hij"};
			DataRow newRow2 = table.LoadDataRow (row2, true);
                                                                                                                             
			try {
				table.EndLoadData ();
				Fail ("#A01");
			}
			catch (ConstraintException) {
			}
		}
		[Test]
		public void Changes () //To test GetChanges and RejectChanges
		{
			DataTable table = new DataTable ();

			DataColumn col = new DataColumn ();
			col.ColumnName = "Id";
			col.DataType = Type.GetType ("System.Int32");
			table.Columns.Add (col);
			UniqueConstraint uc = new UniqueConstraint ("UK1", table.Columns[0] );
			table.Constraints.Add (uc);
                                                                                                                             
			col = new DataColumn ();
			col.ColumnName = "Name";
			col.DataType = Type.GetType ("System.String");
			table.Columns.Add (col);			

			DataRow row = table.NewRow ();
			row ["Id"] = 147;
			row ["name"] = "Abc";
			table.Rows.Add (row);
			table.AcceptChanges ();
                        
			row = table.NewRow ();
			row ["Id"] = 47;
			row ["name"] = "Efg";
			table.Rows.Add (row);

			//Testing GetChanges
			DataTable changesTable = table.GetChanges ();
			AssertEquals ("#A01", 1 ,changesTable.Rows.Count);
 			AssertEquals ("#A02","Efg" ,changesTable.Rows[0]["Name"]);               	
			table.AcceptChanges ();
			changesTable = table.GetChanges ();
			try {
				int cnt = changesTable.Rows.Count;
			}
			catch(Exception e) {
				if (e.GetType () != typeof (AssertionException))
					AssertEquals ("#A03",typeof(NullReferenceException) ,e.GetType ());
				else
					Console.WriteLine (e);
			}
			
			//Testing RejectChanges
			row = table.NewRow ();
                        row ["Id"] = 247;
                        row ["name"] = "Hij";
                        table.Rows.Add (row);

			(table.Rows [0])["Name"] = "AaBbCc";
			table.RejectChanges ();
			AssertEquals ("#A03", "Abc" , (table.Rows [0]) ["Name"]);
			AssertEquals ("#A04", 2, table.Rows.Count);
		}
		
                [Test]
                public void ImportRowTest ()
                {
                        // build source table
                        DataTable src = new DataTable ();
                        src.Columns.Add ("id", typeof (int));
                        src.Columns.Add ("name", typeof (string));

                        src.PrimaryKey = new DataColumn [] {src.Columns [0]} ;

                        src.Rows.Add (new object [] { 1, "mono 1" });
                        src.Rows.Add (new object [] { 2, "mono 2" });
                        src.Rows.Add (new object [] { 3, "mono 3" });
                        src.AcceptChanges ();

                        src.Rows [0] [1] = "mono changed 1";  // modify 1st row
                        src.Rows [1].Delete ();              // delete 2nd row
                        // 3rd row is unchanged
                        src.Rows.Add (new object [] { 4, "mono 4" }); // add 4th row

                        // build target table
                        DataTable target = new DataTable ();
                        target.Columns.Add ("id", typeof (int));
                        target.Columns.Add ("name", typeof (string));

                        target.PrimaryKey = new DataColumn [] {target.Columns [0]} ;

                        // import all rows
                        target.ImportRow (src.Rows [0]);     // import 1st row
                        target.ImportRow (src.Rows [1]);     // import 2nd row
                        target.ImportRow (src.Rows [2]);     // import 3rd row
                        target.ImportRow (src.Rows [3]);     // import 4th row

                        try {
                                target.ImportRow (src.Rows [2]); // import 3rd row again
                                Fail ("#AA1 Should have thrown exception violativ PK");
                        } catch (ConstraintException e) {}

                        // check row states
                        AssertEquals ("#A1", src.Rows [0].RowState, target.Rows [0].RowState);
                        AssertEquals ("#A2", src.Rows [1].RowState, target.Rows [1].RowState);
                        AssertEquals ("#A3", src.Rows [2].RowState, target.Rows [2].RowState);
                        AssertEquals ("#A4", src.Rows [3].RowState, target.Rows [3].RowState);

                        // check for modified row (1st row)
                        AssertEquals ("#B1", (string) src.Rows [0] [1], (string) target.Rows [0] [1]);
                        AssertEquals ("#B2", (string) src.Rows [0] [1, DataRowVersion.Default], (string) target.Rows [0] [1, DataRowVersion.Default]);
                        AssertEquals ("#B3", (string) src.Rows [0] [1, DataRowVersion.Original], (string) target.Rows [0] [1, DataRowVersion.Original]);
                        AssertEquals ("#B4", (string) src.Rows [0] [1, DataRowVersion.Current], (string) target.Rows [0] [1, DataRowVersion.Current]);
                        AssertEquals ("#B5", false, target.Rows [0].HasVersion(DataRowVersion.Proposed));

                        // check for deleted row (2nd row)
                        AssertEquals ("#C1", (string) src.Rows [1] [1, DataRowVersion.Original], (string) target.Rows [1] [1, DataRowVersion.Original]);

                        // check for unchanged row (3rd row)
                        AssertEquals ("#D1", (string) src.Rows [2] [1], (string) target.Rows [2] [1]);
                        AssertEquals ("#D2", (string) src.Rows [2] [1, DataRowVersion.Default], (string) target.Rows [2] [1, DataRowVersion.Default]);
                        AssertEquals ("#D3", (string) src.Rows [2] [1, DataRowVersion.Original], (string) target.Rows [2] [1, DataRowVersion.Original]);
                        AssertEquals ("#D4", (string) src.Rows [2] [1, DataRowVersion.Current], (string) target.Rows [2] [1, DataRowVersion.Current]);

                        // check for newly added row (4th row)
                        AssertEquals ("#E1", (string) src.Rows [3] [1], (string) target.Rows [3] [1]);
                        AssertEquals ("#E2", (string) src.Rows [3] [1, DataRowVersion.Default], (string) target.Rows [3] [1, DataRowVersion.Default]);
                        AssertEquals ("#E3", (string) src.Rows [3] [1, DataRowVersion.Current], (string) target.Rows [3] [1, DataRowVersion.Current]);
                }

                [Test]
		public void ImportRowDetachedTest ()
		{
			DataTable table = new DataTable ();
			DataColumn col = new DataColumn ();
			col.ColumnName = "Id";
			col.DataType = Type.GetType ("System.Int32");
			table.Columns.Add (col);

                        table.PrimaryKey = new DataColumn [] {col};

                        col = new DataColumn ();
			col.ColumnName = "Name";
			col.DataType = Type.GetType ("System.String");
			table.Columns.Add (col);
                        
			DataRow row = table.NewRow ();
			row ["Id"] = 147;
			row ["name"] = "Abc";

                        // keep silent as ms.net ;-), though this is not useful.
                        table.ImportRow (row);

			//if RowState is detached, then dont import the row.
			AssertEquals ("#1", 0, table.Rows.Count);
		}

		[Test]
		public void ImportRowDeletedTest ()
		{
			DataTable table = new DataTable ();
			table.Columns.Add ("col", typeof (int));
			table.Columns.Add ("col1", typeof (int));

			DataRow row = table.Rows.Add (new object[] {1,2});
			table.PrimaryKey = new DataColumn[] {table.Columns[0]};
			table.AcceptChanges ();

			// If row is in Deleted state, then ImportRow loads the
			// row.
			row.Delete ();
			table.ImportRow (row);
			AssertEquals ("#1", 2, table.Rows.Count);

			// Both the deleted rows shud be now gone
			table.AcceptChanges ();
			AssertEquals ("#2", 0, table.Rows.Count);

			//just add another row
			row = table.Rows.Add (new object[] {1,2});
			// no exception shud be thrown
			table.AcceptChanges ();

			// If row is in Deleted state, then ImportRow loads the
			// row and validate only on RejectChanges
			row.Delete ();
			table.ImportRow (row);
			AssertEquals ("#3", 2, table.Rows.Count);
			AssertEquals ("#4", DataRowState.Deleted, table.Rows[1].RowState);

			try {
				table.RejectChanges ();
				Fail ("#5");
			} catch (ConstraintException e) {
			}
		}

		[Test]
		public void ClearReset () //To test Clear and Reset methods
		{
			DataTable table = new DataTable ("table");
			DataTable table1 = new DataTable ("table1");
                
			DataSet set = new DataSet ();
			set.Tables.Add (table);
			set.Tables.Add (table1);

                        table.Columns.Add ("Id", typeof (int));
                        table.Columns.Add ("Name", typeof (string));
                        table.Constraints.Add (new UniqueConstraint ("UK1", table.Columns [0]));
                        table.CaseSensitive = false;
                        
                        table1.Columns.Add ("Id", typeof (int));
                        table1.Columns.Add ("Name", typeof (string));

                        DataRelation dr = new DataRelation ("DR", table.Columns[0], table1.Columns[0]);
			set.Relations.Add (dr);
                
			DataRow row = table.NewRow ();
			row ["Id"] = 147;
			row ["name"] = "Roopa";
			table.Rows.Add (row);
                
			row = table.NewRow ();
			row ["Id"] = 47;
			row ["Name"] = "roopa";
			table.Rows.Add (row);
                
			AssertEquals (2, table.Rows.Count);
			AssertEquals (1, table.ChildRelations.Count);
			try {
				table.Reset ();
				Fail ("#A01, should have thrown ArgumentException");
			}
                        catch (ArgumentException) {
			}
			AssertEquals ("#CT01", 0, table.Rows.Count);
			AssertEquals ("#CT02", 0, table.ChildRelations.Count);
			AssertEquals ("#CT03", 0, table.ParentRelations.Count);
			AssertEquals ("#CT04", 0, table.Constraints.Count);

			table1.Reset ();
			AssertEquals ("#A05", 0, table1.Rows.Count);
			AssertEquals ("#A06", 0, table1.Constraints.Count);
			AssertEquals ("#A07", 0, table1.ParentRelations.Count);
		
                        // clear test
			table.Clear ();
			AssertEquals ("#A08", 0, table.Rows.Count);
#if NET_1_1
			AssertEquals ("#A09", 0, table.Constraints.Count);
#else
			AssertEquals ("#A09", 1, table.Constraints.Count);
#endif
			AssertEquals ("#A10", 0, table.ChildRelations.Count);

		}

                [Test]
                public void ClearTest ()
                {
                        DataTable table = new DataTable ("test");
                        table.Columns.Add ("id", typeof (int));
                        table.Columns.Add ("name", typeof (string));
                        
                        table.PrimaryKey = new DataColumn [] { table.Columns [0] } ;
                        
                        table.Rows.Add (new object [] { 1, "mono 1" });
                        table.Rows.Add (new object [] { 2, "mono 2" });
                        table.Rows.Add (new object [] { 3, "mono 3" });
                        table.Rows.Add (new object [] { 4, "mono 4" });

                        table.AcceptChanges ();
#if NET_2_0
                        _tableClearedEventFired = false;
                        table.TableCleared += new DataTableClearEventHandler (OnTableCleared);
#endif // NET_2_0
                        
                        table.Clear ();
#if NET_2_0
                        AssertEquals ("#0 should have fired cleared event", true, _tableClearedEventFired);
#endif // NET_2_0
                        
                        DataRow r = table.Rows.Find (1);
                        AssertEquals ("#1 should have cleared", true, r == null);

                        // try adding new row. indexes should have cleared
                        table.Rows.Add (new object [] { 2, "mono 2" });
                        AssertEquals ("#2 should add row", 1, table.Rows.Count);
                }
#if NET_2_0
                private bool _tableClearedEventFired = false;
                private void OnTableCleared (object src, DataTableClearEventArgs args)
                {
                        _tableClearedEventFired = true;
                }
#endif // NET_2_0
                

		[Test]
		public void Serialize ()
		{
			MemoryStream fs = new MemoryStream ();
			
			// Construct a BinaryFormatter and use it 
			// to serialize the data to the stream.
			BinaryFormatter formatter = new BinaryFormatter();
		
			// Create an array with multiple elements refering to 
			// the one Singleton object.
			DataTable dt = new DataTable();
		
		
			dt.Columns.Add(new DataColumn("Id", typeof(string)));
			dt.Columns.Add(new DataColumn("ContactName", typeof(string)));
			dt.Columns.Add(new DataColumn("ContactTitle", typeof(string)));
			dt.Columns.Add(new DataColumn("ContactAreaCode", typeof(string)));
			dt.Columns.Add(new DataColumn("ContactPhone", typeof(string)));
		
			DataRow loRowToAdd;
			loRowToAdd = dt.NewRow();
			loRowToAdd[0] = "a";
			loRowToAdd[1] = "b";
			loRowToAdd[2] = "c";
			loRowToAdd[3] = "d";
			loRowToAdd[4] = "e";
						
			dt.Rows.Add(loRowToAdd);
		
			DataTable[] dtarr = new DataTable[] {dt}; 
		
			// Serialize the array elements.
			formatter.Serialize(fs, dtarr);
		
			// Deserialize the array elements.
			fs.Position = 0;
			DataTable[] a2 = (DataTable[]) formatter.Deserialize(fs);
		
			DataSet ds = new DataSet();
			ds.Tables.Add(a2[0]);
		
			StringWriter sw = new StringWriter ();
			ds.WriteXml(sw);
			XmlDocument doc = new XmlDocument ();
			doc.LoadXml (sw.ToString ());
			AssertEquals (5, doc.DocumentElement.FirstChild.ChildNodes.Count);
		}

		[Test]
		[ExpectedException (typeof (DataException))]
		public void SetPrimaryKeyAssertsNonNull ()
		{
			DataTable dt = new DataTable ("table");
			dt.Columns.Add ("col1");
			dt.Columns.Add ("col2");
			dt.Constraints.Add (new UniqueConstraint (dt.Columns [0]));
			dt.Rows.Add (new object [] {1, 3});
			dt.Rows.Add (new object [] {DBNull.Value, 3});

			dt.PrimaryKey = new DataColumn [] {dt.Columns [0]};
		}

		[Test]
		[ExpectedException (typeof (NoNullAllowedException))]
		public void PrimaryKeyColumnChecksNonNull ()
		{
			DataTable dt = new DataTable ("table");
			dt.Columns.Add ("col1");
			dt.Columns.Add ("col2");
			dt.Constraints.Add (new UniqueConstraint (dt.Columns [0]));
			dt.PrimaryKey = new DataColumn [] {dt.Columns [0]};
			dt.Rows.Add (new object [] {1, 3});
			dt.Rows.Add (new object [] {DBNull.Value, 3});
		}

		[Test]
		public void PrimaryKey_CheckSetsAllowDBNull ()
		{
			DataTable table = new DataTable ();
			DataColumn col1 = table.Columns.Add ("col1", typeof (int));
			DataColumn col2 = table.Columns.Add ("col2", typeof (int));
	
			AssertEquals ("#1" , true, col1.AllowDBNull);
			AssertEquals ("#2" , true, col2.AllowDBNull);
			AssertEquals ("#3" , false, col2.Unique);
			AssertEquals ("#4" , false, col2.Unique);

			table.PrimaryKey = new DataColumn[] {col1,col2};
			AssertEquals ("#5" , false, col1.AllowDBNull);
			AssertEquals ("#6" , false, col2.AllowDBNull);
			// LAMESPEC or bug ?? 
			AssertEquals ("#7" , false, col1.Unique);
			AssertEquals ("#8" , false, col2.Unique);
		}

		void RowChanging (object o, DataRowChangeEventArgs e)
		{
			AssertEquals ("changing.Action", rowChangingExpectedAction, e.Action);
			rowChangingRowChanging = true;
		}

		void RowChanged (object o, DataRowChangeEventArgs e)
		{
			AssertEquals ("changed.Action", rowChangingExpectedAction, e.Action);
			rowChangingRowChanged = true;
		}

		bool rowChangingRowChanging, rowChangingRowChanged;
		DataRowAction rowChangingExpectedAction;

		[Test]
		public void RowChanging ()
		{
			DataTable dt = new DataTable ("table");
			dt.Columns.Add ("col1");
			dt.Columns.Add ("col2");
			dt.RowChanging += new DataRowChangeEventHandler (RowChanging);
			dt.RowChanged += new DataRowChangeEventHandler (RowChanged);
			rowChangingExpectedAction = DataRowAction.Add;
			dt.Rows.Add (new object [] {1, 2});
			Assert ("changing,Added", rowChangingRowChanging);
			Assert ("changed,Added", rowChangingRowChanged);
			rowChangingExpectedAction = DataRowAction.Change;
			dt.Rows [0] [0] = 2;
			Assert ("changing,Changed", rowChangingRowChanging);
			Assert ("changed,Changed", rowChangingRowChanged);
		}

		 [Test]
                public void CloneSubClassTest()
                {
                        MyDataTable dt1 = new MyDataTable();
                        MyDataTable dt = (MyDataTable)(dt1.Clone());
                        AssertEquals("A#01",2,MyDataTable.count);
                }

                DataRowAction rowActionChanging = DataRowAction.Nothing;
                DataRowAction rowActionChanged  = DataRowAction.Nothing;
                [Test]
                public void AcceptChangesTest ()
                {
                        DataTable dt = new DataTable ("test");
                        dt.Columns.Add ("id", typeof (int));
                        dt.Columns.Add ("name", typeof (string));
                        
                        dt.Rows.Add (new object [] { 1, "mono 1" });

                        dt.RowChanged  += new DataRowChangeEventHandler (OnRowChanged);
                        dt.RowChanging += new DataRowChangeEventHandler (OnRowChanging);

                        try {
                                rowActionChanged = rowActionChanging = DataRowAction.Nothing;
                                dt.AcceptChanges ();

                                AssertEquals ("#1 should have fired event and set action to commit",
                                              DataRowAction.Commit, rowActionChanging);
                                AssertEquals ("#2 should have fired event and set action to commit",
                                              DataRowAction.Commit, rowActionChanged);

                        } finally {
                                dt.RowChanged  -= new DataRowChangeEventHandler (OnRowChanged);
                                dt.RowChanging -= new DataRowChangeEventHandler (OnRowChanging);

                        }
                }

				[Test]
				public void ColumnObjectTypeTest() {
					DataTable dt = new DataTable();
					dt.Columns.Add("Series Label", typeof(SqlInt32));
					dt.Rows.Add(new object[] {"sss"});
					AssertEquals(1, dt.Rows.Count);
				}

                public void OnRowChanging (object src, DataRowChangeEventArgs args)
                {
                        rowActionChanging = args.Action;
                }
                
                public void OnRowChanged (object src, DataRowChangeEventArgs args)
                {
                        rowActionChanged = args.Action;
		}


#if NET_2_0
		#region DataTable.CreateDataReader Tests and DataTable.Load Tests

		private DataTable dt;

		private void localSetup () {
			dt = new DataTable ("test");
			dt.Columns.Add ("id", typeof (int));
			dt.Columns.Add ("name", typeof (string));
			dt.PrimaryKey = new DataColumn[] { dt.Columns["id"] };

			dt.Rows.Add (new object[] { 1, "mono 1" });
			dt.Rows.Add (new object[] { 2, "mono 2" });
			dt.Rows.Add (new object[] { 3, "mono 3" });

			dt.AcceptChanges ();
		}

		[Test]
		public void CreateDataReader1 () {
			localSetup ();
			DataTableReader dtr = dt.CreateDataReader ();
			Assert ("HasRows", dtr.HasRows);
			AssertEquals ("CountCols", dt.Columns.Count, dtr.FieldCount);
			int ri = 0;
			while (dtr.Read ()) {
				for (int i = 0; i < dtr.FieldCount; i++) {
					AssertEquals ("RowData-" + ri + "-" + i, dt.Rows[ri][i],
						dtr[i]);
				}
				ri++;
			}
		}

		[Test]
		public void CreateDataReader2 () {
			localSetup ();
			DataTableReader dtr = dt.CreateDataReader ();
			Assert ("HasRows", dtr.HasRows);
			AssertEquals ("CountCols", dt.Columns.Count, dtr.FieldCount);
			dtr.Read ();
			AssertEquals ("RowData0-0", 1, dtr[0]);
			AssertEquals ("RowData0-1", "mono 1", dtr[1]);
			dtr.Read ();
			AssertEquals ("RowData1-0", 2, dtr[0]);
			AssertEquals ("RowData1-1", "mono 2", dtr[1]);
			dtr.Read ();
			AssertEquals ("RowData2-0", 3, dtr[0]);
			AssertEquals ("RowData2-1", "mono 3", dtr[1]);
		}

		[Test]
		public void Load_Basic () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadBasic");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.Columns["id"].ReadOnly = true;
			dtLoad.Columns["name"].ReadOnly = true;
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1, "load 1" });
			dtLoad.Rows.Add (new object[] { 2, "load 2" });
			dtLoad.Rows.Add (new object[] { 3, "load 3" });
			dtLoad.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr);
			AssertEquals ("NColumns", 2, dtLoad.Columns.Count);
			AssertEquals ("NRows", 3, dtLoad.Rows.Count);
			AssertEquals ("RowData0-0", 1, dtLoad.Rows[0][0]);
			AssertEquals ("RowData0-1", "mono 1", dtLoad.Rows[0][1]);
			AssertEquals ("RowData1-0", 2, dtLoad.Rows[1][0]);
			AssertEquals ("RowData1-1", "mono 2", dtLoad.Rows[1][1]);
			AssertEquals ("RowData2-0", 3, dtLoad.Rows[2][0]);
			AssertEquals ("RowData2-1", "mono 3", dtLoad.Rows[2][1]);
		}

		[Test]
		public void Load_NoSchema () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadNoSchema");
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr);
			AssertEquals ("NColumns", 2, dtLoad.Columns.Count);
			AssertEquals ("NRows", 3, dtLoad.Rows.Count);
			AssertEquals ("RowData0-0", 1, dtLoad.Rows[0][0]);
			AssertEquals ("RowData0-1", "mono 1", dtLoad.Rows[0][1]);
			AssertEquals ("RowData1-0", 2, dtLoad.Rows[1][0]);
			AssertEquals ("RowData1-1", "mono 2", dtLoad.Rows[1][1]);
			AssertEquals ("RowData2-0", 3, dtLoad.Rows[2][0]);
			AssertEquals ("RowData2-1", "mono 3", dtLoad.Rows[2][1]);
		}

		internal struct fillErrorStruct {
			internal string error;
			internal string tableName;
			internal int rowKey;
			internal bool contFlag;
			internal void init (string tbl, int row, bool cont, string err) {
				tableName = tbl;
				rowKey = row;
				contFlag = cont;
				error = err;
			}
		}
		private fillErrorStruct[] fillErr = new fillErrorStruct[3];
		private int fillErrCounter;
		private void fillErrorHandler (object sender, FillErrorEventArgs e) {
			e.Continue = fillErr[fillErrCounter].contFlag;
			AssertEquals ("fillErr-T", fillErr[fillErrCounter].tableName, e.DataTable.TableName);
			AssertEquals ("fillErr-R", fillErr[fillErrCounter].rowKey, e.Values[0]);
			AssertEquals ("fillErr-C", fillErr[fillErrCounter].contFlag, e.Continue);
			AssertEquals ("fillErr-E", fillErr[fillErrCounter].error, e.Errors.Message);
			fillErrCounter++;
		}

		[Test]
		[ExpectedException (typeof (ArgumentException))]
		public void Load_Incompatible () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadIncompatible");
			dtLoad.Columns.Add ("name", typeof (double));
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr);
		}
		[Test]
		[Category ("NotWorking")]
		// Load doesn't have a third overload in System.Data
		// and is commented-out below
		public void Load_IncompatibleEHandlerT () {
			fillErrCounter = 0;
			fillErr[0].init ("LoadIncompatible", 1, true,
				"Input string was not in a correct format.Couldn't store <mono 1> in name Column.  Expected type is Double.");
			fillErr[1].init ("LoadIncompatible", 2, true,
				"Input string was not in a correct format.Couldn't store <mono 2> in name Column.  Expected type is Double.");
			fillErr[2].init ("LoadIncompatible", 3, true,
				"Input string was not in a correct format.Couldn't store <mono 3> in name Column.  Expected type is Double.");
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadIncompatible");
			dtLoad.Columns.Add ("name", typeof (double));
			DataTableReader dtr = dt.CreateDataReader ();
			//dtLoad.Load (dtr,LoadOption.PreserveChanges,fillErrorHandler);
		}
		[Test]
		[Category ("NotWorking")]
		// Load doesn't have a third overload in System.Data
		// and is commented-out below
		[ExpectedException (typeof (ArgumentException))]
		public void Load_IncompatibleEHandlerF () {
			fillErrCounter = 0;
			fillErr[0].init ("LoadIncompatible", 1, false,
				"Input string was not in a correct format.Couldn't store <mono 1> in name Column.  Expected type is Double.");
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadIncompatible");
			dtLoad.Columns.Add ("name", typeof (double));
			DataTableReader dtr = dt.CreateDataReader ();
			//dtLoad.Load (dtr, LoadOption.PreserveChanges, fillErrorHandler);
		}

		[Test]
		public void Load_ExtraColsEqualVal () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadExtraCols");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1 });
			dtLoad.Rows.Add (new object[] { 2 });
			dtLoad.Rows.Add (new object[] { 3 });
			dtLoad.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr);
			AssertEquals ("NColumns", 2, dtLoad.Columns.Count);
			AssertEquals ("NRows", 3, dtLoad.Rows.Count);
			AssertEquals ("RowData0-0", 1, dtLoad.Rows[0][0]);
			AssertEquals ("RowData0-1", "mono 1", dtLoad.Rows[0][1]);
			AssertEquals ("RowData1-0", 2, dtLoad.Rows[1][0]);
			AssertEquals ("RowData1-1", "mono 2", dtLoad.Rows[1][1]);
			AssertEquals ("RowData2-0", 3, dtLoad.Rows[2][0]);
			AssertEquals ("RowData2-1", "mono 3", dtLoad.Rows[2][1]);
		}

		[Test]
		public void Load_ExtraColsNonEqualVal () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadExtraCols");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 4 });
			dtLoad.Rows.Add (new object[] { 5 });
			dtLoad.Rows.Add (new object[] { 6 });
			dtLoad.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr);
			AssertEquals ("NColumns", 2, dtLoad.Columns.Count);
			AssertEquals ("NRows", 6, dtLoad.Rows.Count);
			AssertEquals ("RowData0-0", 4, dtLoad.Rows[0][0]);
			AssertEquals ("RowData1-0", 5, dtLoad.Rows[1][0]);
			AssertEquals ("RowData2-0", 6, dtLoad.Rows[2][0]);
			AssertEquals ("RowData3-0", 1, dtLoad.Rows[3][0]);
			AssertEquals ("RowData3-1", "mono 1", dtLoad.Rows[3][1]);
			AssertEquals ("RowData4-0", 2, dtLoad.Rows[4][0]);
			AssertEquals ("RowData4-1", "mono 2", dtLoad.Rows[4][1]);
			AssertEquals ("RowData5-0", 3, dtLoad.Rows[5][0]);
			AssertEquals ("RowData5-1", "mono 3", dtLoad.Rows[5][1]);
		}

		[Test]
		[ExpectedException (typeof (ConstraintException))]
		public void Load_MissingColsNonNullable () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadMissingCols");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.Columns.Add ("missing", typeof (string));
			dtLoad.Columns["missing"].AllowDBNull = false;
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 4, "mono 4", "miss4" });
			dtLoad.Rows.Add (new object[] { 5, "mono 5", "miss5" });
			dtLoad.Rows.Add (new object[] { 6, "mono 6", "miss6" });
			dtLoad.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr);
		}

		[Test]
		public void Load_MissingColsDefault () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadMissingCols");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.Columns.Add ("missing", typeof (string));
			dtLoad.Columns["missing"].AllowDBNull = false;
			dtLoad.Columns["missing"].DefaultValue = "DefaultValue";
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 4, "mono 4", "miss4" });
			dtLoad.Rows.Add (new object[] { 5, "mono 5", "miss5" });
			dtLoad.Rows.Add (new object[] { 6, "mono 6", "miss6" });
			dtLoad.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr);
			AssertEquals ("NColumns", 3, dtLoad.Columns.Count);
			AssertEquals ("NRows", 6, dtLoad.Rows.Count);
			AssertEquals ("RowData0-0", 4, dtLoad.Rows[0][0]);
			AssertEquals ("RowData0-1", "mono 4", dtLoad.Rows[0][1]);
			AssertEquals ("RowData0-2", "miss4", dtLoad.Rows[0][2]);
			AssertEquals ("RowData1-0", 5, dtLoad.Rows[1][0]);
			AssertEquals ("RowData1-1", "mono 5", dtLoad.Rows[1][1]);
			AssertEquals ("RowData1-2", "miss5", dtLoad.Rows[1][2]);
			AssertEquals ("RowData2-0", 6, dtLoad.Rows[2][0]);
			AssertEquals ("RowData2-1", "mono 6", dtLoad.Rows[2][1]);
			AssertEquals ("RowData2-2", "miss6", dtLoad.Rows[2][2]);
			AssertEquals ("RowData3-0", 1, dtLoad.Rows[3][0]);
			AssertEquals ("RowData3-1", "mono 1", dtLoad.Rows[3][1]);
			AssertEquals ("RowData3-2", "DefaultValue", dtLoad.Rows[3][2]);
			AssertEquals ("RowData4-0", 2, dtLoad.Rows[4][0]);
			AssertEquals ("RowData4-1", "mono 2", dtLoad.Rows[4][1]);
			AssertEquals ("RowData4-2", "DefaultValue", dtLoad.Rows[4][2]);
			AssertEquals ("RowData5-0", 3, dtLoad.Rows[5][0]);
			AssertEquals ("RowData5-1", "mono 3", dtLoad.Rows[5][1]);
			AssertEquals ("RowData5-2", "DefaultValue", dtLoad.Rows[5][2]);
		}

		[Test]
		public void Load_MissingColsNullable () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadMissingCols");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.Columns.Add ("missing", typeof (string));
			dtLoad.Columns["missing"].AllowDBNull = true;
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 4, "mono 4", "miss4" });
			dtLoad.Rows.Add (new object[] { 5, "mono 5", "miss5" });
			dtLoad.Rows.Add (new object[] { 6, "mono 6", "miss6" });
			dtLoad.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr);
			AssertEquals ("NColumns", 3, dtLoad.Columns.Count);
			AssertEquals ("NRows", 6, dtLoad.Rows.Count);
			AssertEquals ("RowData0-0", 4, dtLoad.Rows[0][0]);
			AssertEquals ("RowData0-1", "mono 4", dtLoad.Rows[0][1]);
			AssertEquals ("RowData0-2", "miss4", dtLoad.Rows[0][2]);
			AssertEquals ("RowData1-0", 5, dtLoad.Rows[1][0]);
			AssertEquals ("RowData1-1", "mono 5", dtLoad.Rows[1][1]);
			AssertEquals ("RowData1-2", "miss5", dtLoad.Rows[1][2]);
			AssertEquals ("RowData2-0", 6, dtLoad.Rows[2][0]);
			AssertEquals ("RowData2-1", "mono 6", dtLoad.Rows[2][1]);
			AssertEquals ("RowData2-2", "miss6", dtLoad.Rows[2][2]);
			AssertEquals ("RowData3-0", 1, dtLoad.Rows[3][0]);
			AssertEquals ("RowData3-1", "mono 1", dtLoad.Rows[3][1]);
			//AssertEquals ("RowData3-2", null, dtLoad.Rows[3][2]);
			AssertEquals ("RowData4-0", 2, dtLoad.Rows[4][0]);
			AssertEquals ("RowData4-1", "mono 2", dtLoad.Rows[4][1]);
			//AssertEquals ("RowData4-2", null, dtLoad.Rows[4][2]);
			AssertEquals ("RowData5-0", 3, dtLoad.Rows[5][0]);
			AssertEquals ("RowData5-1", "mono 3", dtLoad.Rows[5][1]);
			//AssertEquals ("RowData5-2", null, dtLoad.Rows[5][2]);
		}

		private DataTable setupRowState () {
			DataTable tbl = new DataTable ("LoadRowStateChanges");
			tbl.RowChanged += new DataRowChangeEventHandler (dtLoad_RowChanged);
			tbl.RowChanging += new DataRowChangeEventHandler (dtLoad_RowChanging);
			tbl.Columns.Add ("id", typeof (int));
			tbl.Columns.Add ("name", typeof (string));
			tbl.PrimaryKey = new DataColumn[] { tbl.Columns["id"] };
			tbl.Rows.Add (new object[] { 1, "RowState 1" });
			tbl.Rows.Add (new object[] { 2, "RowState 2" });
			tbl.Rows.Add (new object[] { 3, "RowState 3" });
			tbl.AcceptChanges ();
			// Update Table with following changes: Row0 unmodified, 
			// Row1 modified, Row2 deleted, Row3 added, Row4 not-present.
			tbl.Rows[1]["name"] = "Modify 2";
			tbl.Rows[2].Delete ();
			DataRow row = tbl.NewRow ();
			row["id"] = 4;
			row["name"] = "Add 4";
			tbl.Rows.Add (row);
			return (tbl);
		}

		private DataRowAction[] rowChangeAction = new DataRowAction[5];
		private bool checkAction = false;
		private int rowChagedCounter, rowChangingCounter;
		private void rowActionInit (DataRowAction[] act) {
			checkAction = true;
			rowChagedCounter = 0;
			rowChangingCounter = 0;
			for (int i = 0; i < 5; i++)
				rowChangeAction[i] = act[i];
		}
		private void rowActionEnd () {
			checkAction = false;
		}
		private void dtLoad_RowChanged (object sender, DataRowChangeEventArgs e) {
			if (checkAction) {
				AssertEquals ("RowChanged" + rowChagedCounter,
					rowChangeAction[rowChagedCounter], e.Action);
				rowChagedCounter++;
			}
		}
		private void dtLoad_RowChanging (object sender, DataRowChangeEventArgs e) {
			if (checkAction) {
				AssertEquals ("RowChanging" + rowChangingCounter,
					rowChangeAction[rowChangingCounter], e.Action);
				rowChangingCounter++;
			}
		}

		[Test]
		[Category ("NotWorking")]
		public void Load_RowStateChangesDefault () {
			localSetup ();
			dt.Rows.Add (new object[] { 4, "mono 4" });
			dt.Rows.Add (new object[] { 5, "mono 5" });
			dt.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			DataTable dtLoad = setupRowState ();
			DataRowAction[] dra = new DataRowAction[] {
				DataRowAction.ChangeCurrentAndOriginal,
				DataRowAction.ChangeOriginal,
				DataRowAction.ChangeOriginal,
				DataRowAction.ChangeOriginal,
				DataRowAction.ChangeCurrentAndOriginal};
			rowActionInit (dra);
			dtLoad.Load (dtr);
			rowActionEnd ();
			// asserting Unchanged Row0
			AssertEquals ("RowData0-C", "mono 1",
				dtLoad.Rows[0][1,DataRowVersion.Current]);
			AssertEquals ("RowData0-O", "mono 1",
				dtLoad.Rows[0][1,DataRowVersion.Original]);
			AssertEquals ("RowState0", DataRowState.Unchanged,
				dtLoad.Rows[0].RowState);
			// asserting Modified Row1
			AssertEquals ("RowData1-C", "Modify 2",
				dtLoad.Rows[1][1, DataRowVersion.Current]);
			AssertEquals ("RowData1-O", "mono 2",
				dtLoad.Rows[1][1, DataRowVersion.Original]);
			AssertEquals ("RowState1", DataRowState.Modified,
				dtLoad.Rows[1].RowState);
			// asserting Deleted Row2
			AssertEquals ("RowData1-O", "mono 3",
				dtLoad.Rows[2][1, DataRowVersion.Original]);
			AssertEquals ("RowState2", DataRowState.Deleted,
				dtLoad.Rows[2].RowState);
			// asserting Added Row3
			AssertEquals ("RowData3-C", "Add 4",
				dtLoad.Rows[3][1, DataRowVersion.Current]);
			AssertEquals ("RowData3-O", "mono 4",
				dtLoad.Rows[3][1, DataRowVersion.Original]);
			AssertEquals ("RowState3", DataRowState.Modified,
				dtLoad.Rows[3].RowState);
			// asserting Unpresent Row4
			AssertEquals ("RowData4-C", "mono 5",
				dtLoad.Rows[4][1, DataRowVersion.Current]);
			AssertEquals ("RowData4-O", "mono 5",
				dtLoad.Rows[4][1, DataRowVersion.Original]);
			AssertEquals ("RowState4", DataRowState.Unchanged,
				dtLoad.Rows[4].RowState);
		}

		[Test]
		[ExpectedException (typeof (VersionNotFoundException))]
		[Category ("NotWorking")]
		public void Load_RowStateChangesDefaultDelete () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadRowStateChanges");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1, "RowState 1" });
			dtLoad.Rows.Add (new object[] { 2, "RowState 2" });
			dtLoad.Rows.Add (new object[] { 3, "RowState 3" });
			dtLoad.AcceptChanges ();
			dtLoad.Rows[2].Delete ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr);
			AssertEquals ("RowData2-C", " ",
				dtLoad.Rows[2][1, DataRowVersion.Current]);
		}

		[Test]
		[Category ("NotWorking")]
		public void Load_RowStatePreserveChanges () {
			localSetup ();
			dt.Rows.Add (new object[] { 4, "mono 4" });
			dt.Rows.Add (new object[] { 5, "mono 5" });
			dt.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			DataTable dtLoad = setupRowState ();
			DataRowAction[] dra = new DataRowAction[] {
				DataRowAction.ChangeCurrentAndOriginal,
				DataRowAction.ChangeOriginal,
				DataRowAction.ChangeOriginal,
				DataRowAction.ChangeOriginal,
				DataRowAction.ChangeCurrentAndOriginal};
			rowActionInit (dra);
			dtLoad.Load (dtr, LoadOption.PreserveChanges);
			rowActionEnd ();
			// asserting Unchanged Row0
			AssertEquals ("RowData0-C", "mono 1",
				dtLoad.Rows[0][1, DataRowVersion.Current]);
			AssertEquals ("RowData0-O", "mono 1",
				dtLoad.Rows[0][1, DataRowVersion.Original]);
			AssertEquals ("RowState0", DataRowState.Unchanged,
				dtLoad.Rows[0].RowState);
			// asserting Modified Row1
			AssertEquals ("RowData1-C", "Modify 2",
				dtLoad.Rows[1][1, DataRowVersion.Current]);
			AssertEquals ("RowData1-O", "mono 2",
				dtLoad.Rows[1][1, DataRowVersion.Original]);
			AssertEquals ("RowState1", DataRowState.Modified,
				dtLoad.Rows[1].RowState);
			// asserting Deleted Row2
			AssertEquals ("RowData1-O", "mono 3",
				dtLoad.Rows[2][1, DataRowVersion.Original]);
			AssertEquals ("RowState2", DataRowState.Deleted,
				dtLoad.Rows[2].RowState);
			// asserting Added Row3
			AssertEquals ("RowData3-C", "Add 4",
				dtLoad.Rows[3][1, DataRowVersion.Current]);
			AssertEquals ("RowData3-O", "mono 4",
				dtLoad.Rows[3][1, DataRowVersion.Original]);
			AssertEquals ("RowState3", DataRowState.Modified,
				dtLoad.Rows[3].RowState);
			// asserting Unpresent Row4
			AssertEquals ("RowData4-C", "mono 5",
				dtLoad.Rows[4][1, DataRowVersion.Current]);
			AssertEquals ("RowData4-O", "mono 5",
				dtLoad.Rows[4][1, DataRowVersion.Original]);
			AssertEquals ("RowState4", DataRowState.Unchanged,
				dtLoad.Rows[4].RowState);
		}

		[Test]
		[ExpectedException (typeof (VersionNotFoundException))]
		[Category ("NotWorking")]
		public void Load_RowStatePreserveChangesDelete () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadRowStateChanges");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1, "RowState 1" });
			dtLoad.Rows.Add (new object[] { 2, "RowState 2" });
			dtLoad.Rows.Add (new object[] { 3, "RowState 3" });
			dtLoad.AcceptChanges ();
			dtLoad.Rows[2].Delete ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr,LoadOption.PreserveChanges);
			AssertEquals ("RowData2-C", " ",
				dtLoad.Rows[2][1, DataRowVersion.Current]);
		}

		[Test]
		[Category ("NotWorking")]
		public void Load_RowStateOverwriteChanges () {
			localSetup ();
			dt.Rows.Add (new object[] { 4, "mono 4" });
			dt.Rows.Add (new object[] { 5, "mono 5" });
			dt.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			DataTable dtLoad = setupRowState ();
			DataRowAction[] dra = new DataRowAction[] {
				DataRowAction.ChangeCurrentAndOriginal,
				DataRowAction.ChangeCurrentAndOriginal,
				DataRowAction.ChangeCurrentAndOriginal,
				DataRowAction.ChangeCurrentAndOriginal,
				DataRowAction.ChangeCurrentAndOriginal};
			rowActionInit (dra);
			dtLoad.Load (dtr, LoadOption.OverwriteChanges);
			rowActionEnd ();
			// asserting Unchanged Row0
			AssertEquals ("RowData0-C", "mono 1",
				dtLoad.Rows[0][1, DataRowVersion.Current]);
			AssertEquals ("RowData0-O", "mono 1",
				dtLoad.Rows[0][1, DataRowVersion.Original]);
			AssertEquals ("RowState0", DataRowState.Unchanged,
				dtLoad.Rows[0].RowState);
			// asserting Modified Row1
			AssertEquals ("RowData1-C", "mono 2",
				dtLoad.Rows[1][1, DataRowVersion.Current]);
			AssertEquals ("RowData1-O", "mono 2",
				dtLoad.Rows[1][1, DataRowVersion.Original]);
			AssertEquals ("RowState1", DataRowState.Unchanged,
				dtLoad.Rows[1].RowState);
			// asserting Deleted Row2
			AssertEquals ("RowData1-C", "mono 3",
			        dtLoad.Rows[2][1, DataRowVersion.Current]);
			AssertEquals ("RowData1-O", "mono 3",
				dtLoad.Rows[2][1, DataRowVersion.Original]);
			AssertEquals ("RowState2", DataRowState.Unchanged,
				dtLoad.Rows[2].RowState);
			// asserting Added Row3
			AssertEquals ("RowData3-C", "mono 4",
				dtLoad.Rows[3][1, DataRowVersion.Current]);
			AssertEquals ("RowData3-O", "mono 4",
				dtLoad.Rows[3][1, DataRowVersion.Original]);
			AssertEquals ("RowState3", DataRowState.Unchanged,
				dtLoad.Rows[3].RowState);
			// asserting Unpresent Row4
			AssertEquals ("RowData4-C", "mono 5",
				dtLoad.Rows[4][1, DataRowVersion.Current]);
			AssertEquals ("RowData4-O", "mono 5",
				dtLoad.Rows[4][1, DataRowVersion.Original]);
			AssertEquals ("RowState4", DataRowState.Unchanged,
				dtLoad.Rows[4].RowState);
		}

		[Test]
		[Category ("NotWorking")]
		public void Load_RowStateUpsert () {
			localSetup ();
			dt.Rows.Add (new object[] { 4, "mono 4" });
			dt.Rows.Add (new object[] { 5, "mono 5" });
			dt.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			DataTable dtLoad = setupRowState ();
			// Notice rowChange-Actions only occur 5 times, as number 
			// of actual rows, ignoring row duplication of the deleted row.
			DataRowAction[] dra = new DataRowAction[] {
				DataRowAction.Change,
				DataRowAction.Change,
				DataRowAction.Add,
				DataRowAction.Change,
				DataRowAction.Add};
			rowActionInit (dra);
			dtLoad.Load (dtr, LoadOption.Upsert);
			rowActionEnd ();
			// asserting Unchanged Row0
			AssertEquals ("RowData0-C", "mono 1",
				dtLoad.Rows[0][1, DataRowVersion.Current]);
			AssertEquals ("RowData0-O", "RowState 1",
				dtLoad.Rows[0][1, DataRowVersion.Original]);
			AssertEquals ("RowState0", DataRowState.Modified,
				dtLoad.Rows[0].RowState);
			// asserting Modified Row1
			AssertEquals ("RowData1-C", "mono 2",
				dtLoad.Rows[1][1, DataRowVersion.Current]);
			AssertEquals ("RowData1-O", "RowState 2",
				dtLoad.Rows[1][1, DataRowVersion.Original]);
			AssertEquals ("RowState1", DataRowState.Modified,
				dtLoad.Rows[1].RowState);
			// asserting Deleted Row2 and "Deleted-Added" Row4
			AssertEquals ("RowData2-O", "RowState 3",
				dtLoad.Rows[2][1, DataRowVersion.Original]);
			AssertEquals ("RowState2", DataRowState.Deleted,
				dtLoad.Rows[2].RowState);
			AssertEquals ("RowData4-C", "mono 3",
				dtLoad.Rows[4][1, DataRowVersion.Current]);
			AssertEquals ("RowState4", DataRowState.Added,
				dtLoad.Rows[4].RowState);
			// asserting Added Row3
			AssertEquals ("RowData3-C", "mono 4",
				dtLoad.Rows[3][1, DataRowVersion.Current]);
			AssertEquals ("RowState3", DataRowState.Added,
				dtLoad.Rows[3].RowState);
			// asserting Unpresent Row5
			// Notice row4 is used for added row of deleted row2 and so
			// unpresent row4 moves to row5
			AssertEquals ("RowData5-C", "mono 5",
				dtLoad.Rows[5][1, DataRowVersion.Current]);
			AssertEquals ("RowState5", DataRowState.Added,
				dtLoad.Rows[5].RowState);
		}

		[Test]
		[Category ("NotWorking")]
		public void Load_RowStateUpsertDuplicateKey1 () {
			localSetup ();
			dt.Rows.Add (new object[] { 4, "mono 4" });
			DataTable dtLoad = new DataTable ("LoadRowStateChanges");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1, "RowState 1" });
			dtLoad.Rows.Add (new object[] { 2, "RowState 2" });
			dtLoad.Rows.Add (new object[] { 3, "RowState 3" });
			dtLoad.AcceptChanges ();
			dtLoad.Rows[2].Delete ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr, LoadOption.Upsert);
			dtLoad.Rows[3][1] = "NEWVAL";
			AssertEquals ("A-RowState2", DataRowState.Deleted,
				dtLoad.Rows[2].RowState);
			AssertEquals ("A-RowData2-id", 3,
				dtLoad.Rows[2][0, DataRowVersion.Original]);
			AssertEquals ("A-RowData2-name", "RowState 3",
				dtLoad.Rows[2][1, DataRowVersion.Original]);
			AssertEquals ("A-RowState3", DataRowState.Added,
				dtLoad.Rows[3].RowState);
			AssertEquals ("A-RowData3-id", 3,
				dtLoad.Rows[3][0, DataRowVersion.Current]);
			AssertEquals ("A-RowData3-name", "NEWVAL",
				dtLoad.Rows[3][1, DataRowVersion.Current]);
			AssertEquals ("A-RowState4", DataRowState.Added,
				dtLoad.Rows[4].RowState);
			AssertEquals ("A-RowData4-id", 4,
				dtLoad.Rows[4][0, DataRowVersion.Current]);
			AssertEquals ("A-RowData4-name", "mono 4",
				dtLoad.Rows[4][1, DataRowVersion.Current]);

			dtLoad.AcceptChanges ();

			AssertEquals ("B-RowState2", DataRowState.Unchanged,
				dtLoad.Rows[2].RowState);
			AssertEquals ("B-RowData2-id", 3,
				dtLoad.Rows[2][0, DataRowVersion.Current]);
			AssertEquals ("B-RowData2-name", "NEWVAL",
				dtLoad.Rows[2][1, DataRowVersion.Current]);
			AssertEquals ("B-RowState3", DataRowState.Unchanged,
				dtLoad.Rows[3].RowState);
			AssertEquals ("B-RowData3-id", 4,
				dtLoad.Rows[3][0, DataRowVersion.Current]);
			AssertEquals ("B-RowData3-name", "mono 4",
				dtLoad.Rows[3][1, DataRowVersion.Current]);
		}

		[Test]
		[ExpectedException (typeof (IndexOutOfRangeException))]
		[Category ("NotWorking")]
		public void Load_RowStateUpsertDuplicateKey2 () {
			localSetup ();
			dt.Rows.Add (new object[] { 4, "mono 4" });
			DataTable dtLoad = new DataTable ("LoadRowStateChanges");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1, "RowState 1" });
			dtLoad.Rows.Add (new object[] { 2, "RowState 2" });
			dtLoad.Rows.Add (new object[] { 3, "RowState 3" });
			dtLoad.AcceptChanges ();
			dtLoad.Rows[2].Delete ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr, LoadOption.Upsert);
			dtLoad.AcceptChanges ();
			AssertEquals ("RowData4", " ", dtLoad.Rows[4][1]);
		}

		[Test]
		[ExpectedException (typeof (VersionNotFoundException))]
		[Category ("NotWorking")]
		public void Load_RowStateUpsertDelete1 () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadRowStateChanges");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1, "RowState 1" });
			dtLoad.Rows.Add (new object[] { 2, "RowState 2" });
			dtLoad.Rows.Add (new object[] { 3, "RowState 3" });
			dtLoad.AcceptChanges ();
			dtLoad.Rows[2].Delete ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr, LoadOption.Upsert);
			AssertEquals ("RowData2-C", " ",
				dtLoad.Rows[2][1, DataRowVersion.Current]);
		}

		[Test]
		[ExpectedException (typeof (VersionNotFoundException))]
		[Category ("NotWorking")]
		public void Load_RowStateUpsertDelete2 () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadRowStateChanges");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1, "RowState 1" });
			dtLoad.Rows.Add (new object[] { 2, "RowState 2" });
			dtLoad.Rows.Add (new object[] { 3, "RowState 3" });
			dtLoad.AcceptChanges ();
			dtLoad.Rows[2].Delete ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr, LoadOption.Upsert);
			AssertEquals ("RowData3-O", " ",
				dtLoad.Rows[3][1, DataRowVersion.Original]);
		}

		[Test]
		[ExpectedException (typeof (VersionNotFoundException))]
		public void Load_RowStateUpsertAdd () {
			localSetup ();
			dt.Rows.Add (new object[] { 4, "mono 4" });
			DataTable dtLoad = new DataTable ("LoadRowStateChanges");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1, "RowState 1" });
			dtLoad.Rows.Add (new object[] { 2, "RowState 2" });
			dtLoad.Rows.Add (new object[] { 3, "RowState 3" });
			dtLoad.AcceptChanges ();
			DataRow row = dtLoad.NewRow ();
			row["id"] = 4;
			row["name"] = "Add 4";
			dtLoad.Rows.Add (row);
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr, LoadOption.Upsert);
			AssertEquals ("RowData3-O", " ",
				dtLoad.Rows[3][1, DataRowVersion.Original]);
		}

		[Test]
		[ExpectedException (typeof (VersionNotFoundException))]
		public void Load_RowStateUpsertUnpresent () {
			localSetup ();
			dt.Rows.Add (new object[] { 4, "mono 4" });
			DataTable dtLoad = new DataTable ("LoadRowStateChanges");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1, "RowState 1" });
			dtLoad.Rows.Add (new object[] { 2, "RowState 2" });
			dtLoad.Rows.Add (new object[] { 3, "RowState 3" });
			dtLoad.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			dtLoad.Load (dtr, LoadOption.Upsert);
			AssertEquals ("RowData3-O", " ",
				dtLoad.Rows[3][1, DataRowVersion.Original]);
		}

		[Test]
		public void Load_RowStateUpsertUnchangedEqualVal () {
			localSetup ();
			DataTable dtLoad = new DataTable ("LoadRowStateChanges");
			dtLoad.Columns.Add ("id", typeof (int));
			dtLoad.Columns.Add ("name", typeof (string));
			dtLoad.PrimaryKey = new DataColumn[] { dtLoad.Columns["id"] };
			dtLoad.Rows.Add (new object[] { 1, "mono 1" });
			dtLoad.AcceptChanges ();
			DataTableReader dtr = dt.CreateDataReader ();
			DataRowAction[] dra = new DataRowAction[] {
				DataRowAction.Nothing,// REAL action
				DataRowAction.Nothing,// dummy  
				DataRowAction.Nothing,// dummy  
				DataRowAction.Nothing,// dummy  
				DataRowAction.Nothing};// dummy  
			rowActionInit (dra);
			dtLoad.Load (dtr, LoadOption.Upsert);
			rowActionEnd ();
			AssertEquals ("RowData0-C", "mono 1",
				dtLoad.Rows[0][1, DataRowVersion.Current]);
			AssertEquals ("RowData0-O", "mono 1",
				dtLoad.Rows[0][1, DataRowVersion.Original]);
			AssertEquals ("RowState0", DataRowState.Unchanged,
				dtLoad.Rows[0].RowState);
		}

		[Test]
		public void LoadDataRow_LoadOptions () {
			// LoadDataRow is covered in detail (without LoadOptions) in DataTableTest2
			// LoadOption tests are covered in detail in DataTable.Load().
			// Therefore only minimal tests of LoadDataRow with LoadOptions are covered here.
			DataTable dt;
			DataRow dr;
			dt = CreateDataTableExample ();
			dt.PrimaryKey = new DataColumn[] { dt.Columns[0] };	//add ParentId as Primary Key
			dt.Columns["String1"].DefaultValue = "Default";

			dr = dt.Select ("ParentId=1")[0];

			//Update existing row with LoadOptions = OverwriteChanges
			dt.BeginLoadData ();
			dt.LoadDataRow (new object[] { 1, null, "Changed" },
				LoadOption.OverwriteChanges);
			dt.EndLoadData ();

			// LoadDataRow(update1) - check column String2
			AssertEquals ("DT72-C", "Changed",
				dr["String2", DataRowVersion.Current]);
			AssertEquals ("DT72-O", "Changed",
				dr["String2", DataRowVersion.Original]);

			// LoadDataRow(update1) - check row state
			AssertEquals ("DT73-LO", DataRowState.Unchanged, dr.RowState);

			//Add New row with LoadOptions = Upsert
			dt.BeginLoadData ();
			dt.LoadDataRow (new object[] { 99, null, "Changed" },
				LoadOption.Upsert);
			dt.EndLoadData ();

			// LoadDataRow(insert1) - check column String2
			dr = dt.Select ("ParentId=99")[0];
			AssertEquals ("DT75-C", "Changed",
				dr["String2", DataRowVersion.Current]);

			// LoadDataRow(insert1) - check row state
			AssertEquals ("DT76-LO", DataRowState.Added, dr.RowState);
		}

		public static DataTable CreateDataTableExample () {
			DataTable dtParent = new DataTable ("Parent");

			dtParent.Columns.Add ("ParentId", typeof (int));
			dtParent.Columns.Add ("String1", typeof (string));
			dtParent.Columns.Add ("String2", typeof (string));

			dtParent.Columns.Add ("ParentDateTime", typeof (DateTime));
			dtParent.Columns.Add ("ParentDouble", typeof (double));
			dtParent.Columns.Add ("ParentBool", typeof (bool));

			dtParent.Rows.Add (new object[] { 1, "1-String1", "1-String2", new DateTime (2005, 1, 1, 0, 0, 0, 0), 1.534, true });
			dtParent.Rows.Add (new object[] { 2, "2-String1", "2-String2", new DateTime (2004, 1, 1, 0, 0, 0, 1), -1.534, true });
			dtParent.Rows.Add (new object[] { 3, "3-String1", "3-String2", new DateTime (2003, 1, 1, 0, 0, 1, 0), double.MinValue * 10000, false });
			dtParent.Rows.Add (new object[] { 4, "4-String1", "4-String2", new DateTime (2002, 1, 1, 0, 1, 0, 0), double.MaxValue / 10000, true });
			dtParent.Rows.Add (new object[] { 5, "5-String1", "5-String2", new DateTime (2001, 1, 1, 1, 0, 0, 0), 0.755, true });
			dtParent.Rows.Add (new object[] { 6, "6-String1", "6-String2", new DateTime (2000, 1, 1, 0, 0, 0, 0), 0.001, false });
			dtParent.AcceptChanges ();
			return dtParent;
		}

		#endregion // DataTable.CreateDataReader Tests and DataTable.Load Tests
#endif // NET_2_0

	}
                                                                                                    
                                                                                                    
         public  class MyDataTable:DataTable {
                                                                                                    
             public static int count = 0;
                                                                                                    
             public MyDataTable() {
                                                                                                    
                    count++;
             }
                                                                                                    
         }

	[Serializable]
	[TestFixture]
	public class AppDomainsAndFormatInfo
	{
		public void Remote ()
		{
			int n = (int) Convert.ChangeType ("5", typeof (int));
			Assertion.AssertEquals ("n", 5, n);
		}
#if !TARGET_JVM
		[Test]
		public void NFIFromBug55978 ()
		{
			AppDomain domain = AppDomain.CreateDomain ("testdomain");
			AppDomainsAndFormatInfo test = new AppDomainsAndFormatInfo ();
			test.Remote ();
			domain.DoCallBack (new CrossAppDomainDelegate (test.Remote));
			AppDomain.Unload (domain);
		}
#endif

		[Test]
		public void Bug55978 ()
		{
			DataTable dt = new DataTable ();
			dt.Columns.Add ("StartDate", typeof (DateTime));
	 
			DataRow dr;
			DateTime date = DateTime.Now;
	 
			for (int i = 0; i < 10; i++) {
				dr = dt.NewRow ();
				dr ["StartDate"] = date.AddDays (i);
				dt.Rows.Add (dr);
			}
	 
			DataView dv = dt.DefaultView;
			dv.RowFilter = "StartDate >= '" + DateTime.Now.AddDays (2) + "' and StartDate <= '" + DateTime.Now.AddDays (4) + "'";
			Assertion.AssertEquals ("Table", 10, dt.Rows.Count);
			Assertion.AssertEquals ("View", 2, dv.Count);
		}
	}
}
