﻿/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 28.06.2009
 * Zeit: 18:54
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Interfaces;
using SimpleExpressionEvaluator;
using SimpleExpressionEvaluator.Evaluation;
using SimpleExpressionEvaluator.Utilities;

namespace ICSharpCode.Reports.Expressions.ReportingLanguage
{
	/// <summary>
	/// Description of ExpressionEvaluatorFassade.
	/// </summary>
	public class ExpressionEvaluatorFacade:IExpressionEvaluatorFacade
	{
		private ReportingLanguageCompiler compiler;
		private ExpressionContext context;
		private IPageInfo singlePage;
		 
		
		public ExpressionEvaluatorFacade(IPageInfo pageInfo)
		{
			compiler = new ReportingLanguageCompiler();
			this.context = new ExpressionContext(null);
			context.ResolveUnknownVariable += VariableStore;
			context.ResolveMissingFunction += FunctionStore;
			SinglePage = pageInfo;
			compiler.SinglePage = pageInfo;
		}
		
		
		public string Evaluate (string expression)
		{
			if (CanEvaluate(expression)) {
				IExpression compiled = compiler.CompileExpression<string>(expression);
				this.context.ContextObject = this.SinglePage;
				if (compiled != null) {
					return (compiled.Evaluate(context)).ToString();
				}
			}
			return expression;
		}
		
		
		private static bool CanEvaluate (string expressionn)
		{
			if ((!String.IsNullOrEmpty(expressionn)) && (expressionn.StartsWith("="))) {
				return true;
			}
			return false;
		}
		
		
		private void FunctionStore (object sender,SimpleExpressionEvaluator.Evaluation.UnknownFunctionEventArgs e)
		{
			
			PropertyPath path = null;
			object searchObj = null;
			
			path = e.ContextObject.ParsePropertyPath(e.FunctionName);
			if (path != null) {
				searchObj = e.ContextObject;
			} else {
				throw new UnknownFunctionException(e.FunctionName);
			}
			e.Function = functionArgs => path.Evaluate(searchObj);
		}
		
		
		private void VariableStore (object sender,SimpleExpressionEvaluator.Evaluation.UnknownVariableEventArgs e)
		{
			
			PropertyPath path = this.singlePage.ParsePropertyPath(e.VariableName);
			if (path != null) {
				e.VariableValue = path.Evaluate(path);
			}
			// Look in Parametershash
			if (singlePage.ParameterHash.ContainsKey(e.VariableName)) {
				e.VariableValue = singlePage.ParameterHash[e.VariableName].ToString();
			}
		}
		
		
		public IPageInfo SinglePage {
			get { return singlePage; }
			set {
				singlePage = value;
				this.compiler.SinglePage = singlePage;
			}
		}
	}
}
