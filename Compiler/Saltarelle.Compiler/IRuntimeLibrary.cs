﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.NRefactory.TypeSystem;
using Saltarelle.Compiler.JSModel.Expressions;

namespace Saltarelle.Compiler {
	public enum TypeContext {
		/// <summary>
		/// A constructor for the type is being invoked, either through an instantiation or through constructor chaining. Note that for static method constructor, <see cref="UseStaticMember"/> is used instead.
		/// </summary>
		InvokeConstructor,

		/// <summary>
		/// The type is being used as a generic argument.
		/// </summary>
		GenericArgument,

		/// <summary>
		/// The type is a target of a typeof expression.
		/// </summary>
		TypeOf,

		/// <summary>
		/// The type is being inherited from. A null result means that the type should not appear in the inheritance list.
		/// </summary>
		Inheritance,

		/// <summary>
		/// The type is being cast to. A null result means that the cast should be replaced with a regular assignment.
		/// </summary>
		CastTarget,

		/// <summary>
		/// The type is being used in a default(T) expression.
		/// </summary>
		GetDefaultValue,

		/// <summary>
		/// The type is being used to invoke one of its static methods.
		/// </summary>
		UseStaticMember,

		/// <summary>
		/// The type is being used because a derived class uses the 'base' keyword.
		/// </summary>
		BindBaseCall,
	}

	public interface IRuntimeLibrary {
		/// <summary>
		/// Returns an expression that references a type. This might mean a simple name, a generic instantiation, or something else.
		/// </summary>
		/// <param name="type">Type to return an expression for.</param>
		/// <param name="context">The context for which the type is desired.</param>
		JsExpression GetScriptType(IType type, TypeContext context);

		/// <summary>
		/// Returns an expression that determines if an expression is of a type (equivalent to C# "is").
		/// This might also represent an unboxing, in which case it must be verified that (any non-null) object can be converted to the target type before returning true.
		/// </summary>
		JsExpression TypeIs(JsExpression expression, IType sourceType, IType targetType);

		/// <summary>
		/// Returns an expression that casts an expression to a specified type, or returns null if the expression is not of that type (equivalent to C# "as").
		/// This might also represent an unboxing, in which null should be returned if the object can be converted to the target type (eg, when unboxing an integer it must be verified that there are no decimal places in the number).
		/// </summary>
		JsExpression TryDowncast(JsExpression expression, IType sourceType, IType targetType);

		/// <summary>
		/// Returns an expression that casts a class to a derived class, or throws an exception if the cast is not possible.
		/// This might also represent an unboxing, in which case it must be verified that (any non-null) object can be converted to the target type (eg, when unboxing an integer it must be verified that there are no decimal places in the number).
		/// </summary>
		JsExpression Downcast(JsExpression expression, IType sourceType, IType targetType);

		/// <summary>
		/// Returns an expression that performs an upcast (equivalent to (IList)list, where list is a List). Note that this might also represent a generic variance conversion.
		/// </summary>
		JsExpression Upcast(JsExpression expression, IType sourceType, IType targetType);

		/// <summary>
		/// Returns an expression that determines if two reference values are equal.
		/// </summary>
		JsExpression ReferenceEquals(JsExpression a, JsExpression b);

		/// <summary>
		/// Returns an expression that determines if two reference values are not equal.
		/// </summary>
		JsExpression ReferenceNotEquals(JsExpression a, JsExpression b);

		/// <summary>
		/// Returns an expression that will instantiate a generic method.
		/// </summary>
		JsExpression InstantiateGenericMethod(JsExpression method, IEnumerable<IType> typeArguments);

		/// <summary>
		/// Returns an expression that will convert a given expression to an exception. This is used to be able to throw a JS string and catch it as an Exception.
		/// </summary>
		JsExpression MakeException(JsExpression operand);

		/// <summary>
		/// Returns an expression that will perform integer division.
		/// </summary>
		JsExpression IntegerDivision(JsExpression numerator, JsExpression denominator);

		/// <summary>
		/// Returns an expression that converts a floating-point number to an integer.
		/// </summary>
		JsExpression FloatToInt(JsExpression operand);

		/// <summary>
		/// Returns an expression that will perform null coalesce (C#: a ?? b).
		/// </summary>
		JsExpression Coalesce(JsExpression a, JsExpression b);

		/// <summary>
		/// Generates a lifted version of an expression.
		/// </summary>
		/// <param name="expression">Expression to lift. This will always be an invocation, or a (unary or binary) operator.</param>
		JsExpression Lift(JsExpression expression);

		/// <summary>
		/// Generates an expression that converts from a nullable type to a non-nullable type (should return the passed-in argument if non-null, throw if it is null).
		/// </summary>
		/// <param name="expression">Expression to ensure that it is non-null.</param>
		JsExpression FromNullable(JsExpression expression);

		/// <summary>
		/// Generates a call to the lifted boolean &amp; operator, which has the same semantics as the SQL AND operator.
		/// </summary>
		JsExpression LiftedBooleanAnd(JsExpression a, JsExpression b);

		/// <summary>
		/// Generates a call to the lifted boolean | operator, which has the same semantics as the SQL OR operator.
		/// </summary>
		JsExpression LiftedBooleanOr(JsExpression a, JsExpression b);

		/// <summary>
		/// Bind a function to a target that will become "this" inside the function.
		/// </summary>
		JsExpression Bind(JsExpression function, JsExpression target);

		/// <summary>
		/// Returns an expression that invokes the specified function, but the context ('this') in the Javascript will be the first parameter. Used eg. in the delegate argument to jQuery.each().
		/// </summary>
		JsExpression BindFirstParameterToThis(JsExpression function);

		/// <summary>
		/// Generates an expression that returns the default value for a type (C#: default(T)).
		/// </summary>
		JsExpression Default(IType type);

		/// <summary>
		/// Generates an expression that creates an array of a specified size (one item for each rank), with all elements initialized to their default values.
		/// </summary>
		JsExpression CreateArray(IType elementType, IEnumerable<JsExpression> sizes);

		/// <summary>
		/// Generates an expression that copies an existing delegate to a new one.
		/// </summary>
		JsExpression CloneDelegate(JsExpression source, IType sourceType, IType targetType);

		/// <summary>
		/// Generates an expression to call a base implementation of an overridden method
		/// </summary>
		/// <param name="baseType">Type whose implementation of the method to invoke.</param>
		/// <param name="methodName">Name of the method to invoke.</param>
		/// <param name="typeArguments">Type arguments for the method, or an empty enumerable.</param>
		/// <param name="thisAndArguments">Arguments to the method, including "this" as the first element.</param>
		JsExpression CallBase(IType baseType, string methodName, IList<IType> typeArguments, IEnumerable<JsExpression> thisAndArguments);

		/// <summary>
		/// Generates an expression to bind a base implementation of an overridden method. Used when converting a method group to a delegate.
		/// </summary>
		/// <param name="baseType">Type whose implementation of the method to bind.</param>
		/// <param name="methodName">Name of the method to bind.</param>
		/// <param name="typeArguments">Type arguments for the method, or an empty enumerable.</param>
		/// <param name="@this">Expression to use for "this" (target of the method call).</param>
		JsExpression BindBaseCall(IType baseType, string methodName, IList<IType> typeArguments, JsExpression @this);

		/// <summary>
		/// Generates an object that implements the <see cref="IEnumerator{T}"/> interface using the supplied methods.
		/// </summary>
		/// <param name="yieldType">The yield type of the enumerator. <see cref="object"/> if the enumerator is non-generic.</param>
		/// <param name="moveNext">Function to invoke when <see cref="IEnumerator.MoveNext"/> is invoked on the enumerator.</param>
		/// <param name="getCurrent">Function that returns the current value of the enumerator.</param>
		/// <param name="dispose">Function to invoke when <see cref="IDisposable.Dispose"/> is invoked on the enumerator, or null if no dispose is required.</param>
		JsExpression MakeEnumerator(IType yieldType, JsExpression moveNext, JsExpression getCurrent, JsExpression dispose);

		/// <summary>
		/// Generates an object that implements the <see cref="IEnumerable{T}"/> interface using the supplied methods.
		/// </summary>
		/// <param name="yieldType">The yield type of the enumerable. <see cref="object"/> if the enumerable is non-generic.</param>
		/// <param name="getEnumerator">Function to invoke when <see cref="IEnumerable.GetEnumerator"/> is invoked on the enumerator.</param>
		JsExpression MakeEnumerable(IType yieldType, JsExpression getEnumerator);

		/// <summary>
		/// Generates an expression that gets the value at a specific index of a multi-dimensional array.
		/// </summary>
		JsExpression GetMultiDimensionalArrayValue(JsExpression array, IEnumerable<JsExpression> indices);

		/// <summary>
		/// Generates an expression that sets the value at a specific index of a multi-dimensional array.
		/// </summary>
		JsExpression SetMultiDimensionalArrayValue(JsExpression array, IEnumerable<JsExpression> indices, JsExpression value);

		/// <summary>
		/// Generates an expression that creates a TaskCompletionSource.
		/// </summary>
		/// <param name="taskGenericArgument">If the method being built returns a <c>Task&lt;T&gt;</c>, this parameter will contain <c>T</c>. If the method returns a non-generic task, this parameter will be null.</param>
		JsExpression CreateTaskCompletionSource(IType taskGenericArgument);

		/// <summary>
		/// Generates an expression that applies the result of an async method to a TaskCompletionSource.
		/// </summary>
		/// <param name="taskCompletionSource">The TaskCompletionSource instance used in the method.</param>
		/// <param name="value">Value to return. Will be null if the method does not return a value (in which case it must be a method returning a non-generic task).</param>
		JsExpression SetAsyncResult(JsExpression taskCompletionSource, JsExpression value);

		/// <summary>
		/// Generates an expression that applies an exception to a TaskCompletionSource.
		/// </summary>
		/// <param name="taskCompletionSource">The TaskCompletionSource instance used in the method.</param>
		/// <param name="exception">The exception to return. Note that this may be any object (not necessarily an Exception instance).</param>
		JsExpression SetAsyncException(JsExpression taskCompletionSource, JsExpression exception);

		/// <summary>
		/// Generates an expression that retrieves the Task instance from a TaskCompletionSource.
		/// </summary>
		/// <param name="taskCompletionSource">The TaskCompletionSource instance used in the method.</param>
		JsExpression GetTaskFromTaskCompletionSource(JsExpression taskCompletionSource);
	}
}
