﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using BlackBox.Testing;
using Xunit;

namespace BlackBox.Tests.Testing
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable MemberCanBeMadeStatic.Local
    public class ObjectComparisonTests : BDD<ObjectComparisonTests>
    {
        [Fact]
        public void Comparing_true_and_true_does_not_yield_any_exception()
        {
            Given.We_want_to_compare_two_objects();
            When.We_compare_true_and_true();
            Then.Nothing();
        }

        [Fact]
        public void Comparing_true_and_false_yields_an_exception()
        {
            Given.We_want_to_compare_two_objects();
            When.We_compare_true_and_false();
            Then.We_get_the_appropriate_exception();
        }

        [Fact]
        public void The_mismatch_exception_message_contains_information_about_the_mismatch()
        {
            Given.We_have_mismatch_exception_for_a_specific_comparison();
            When.We_read_the_message_of_the_exception();
            Then.It_tells_us_what_the_difference_is();
        }

        [Fact]
        public void Do_not_follow_circular_references()
        {
            Given.We_want_to_compare_two_objects();
            When.We_compare_two_objects_with_circular_references();
            Then.Nothing();
        }

        [Fact]
        public void We_do_not_want_to_compare_the_Capacity_or_other_properties_when_comparing_IEnumerables()
        {
            Given.We_want_to_compare_two_objects();
            When.We_compare_two_lists_with_equal_objects_but_different_capacity();
            Then.Nothing();
        }

        [Fact]
        public void Comparing_two_lists_of_different_objects_yields_all_nested_differences()
        {
            Given.We_want_to_compare_two_objects();
            When.We_compare_two_lists_with_two_objects_each_that_differ_in_two_properties_each();
            Then.The_exception_message_contains_all_four_differences();
        }

        [Fact]
        public void Can_exclude_a_certain_property_from_a_simple_comparison_using_a_lambda_representation()
        {
            Given.A_lambda_representation_of_a_property_we_wish_to_ignore();
            When.We_use_that_lambda_representation_when_we_compare_two_objects_that_differ_on_that_property();
            Then.Nothing();
        }

        [Fact]
        public void Cannot_ignore_the_root_object()
        {
            Given.A_self_selector();
            When.We_try_to_ignore_on_that_self_selector();
            Then.We_get_an_exception_saying_it_is_not_a_valid_member_expression();
        }

        [Fact]
        public void Cannot_ignore_on_unary_expressions()
        {
            Given.An_unary_selector();
            When.We_try_to_ignore_on_that_unary_selector();
            Then.We_get_an_exception_saying_it_is_not_a_valid_member_expression();
        }

        [Fact]
        public void Ignoring_a_certain_property_means_ignoring_all_differences_in_underlying_properties_aswell()
        {
            Given.A_lamba_representation_of_a_reference_type_property_we_wish_to_ignore();
            When.We_use_that_lambda_representation_when_we_compare_two_objects_that_differ_on_underlying_properties();
            Then.Nothing();
        }

        private void A_lamba_representation_of_a_reference_type_property_we_wish_to_ignore()
        {
            referenceTypePropertySelector = o => o.MySimpleObject;
        }

        private void We_use_that_lambda_representation_when_we_compare_two_objects_that_differ_on_underlying_properties()
        {
            var someObject = new ObjectWithMixedTypeProperties {MySimpleObject = new ObjectWithValueTypeProperties()};
            var anotherObject = new ObjectWithMixedTypeProperties
                                    {
                                        MySimpleObject = new ObjectWithValueTypeProperties
                                                             {
                                                                 MyBoolean = true,
                                                                 MyDecimal = 1,
                                                                 MyInteger = 1
                                                             }
                                    };
            var test = new CharacterizationTest();
            test.IgnoreOnType(referenceTypePropertySelector);
            test.CompareObjects(someObject, anotherObject);
        }

        private void A_self_selector()
        {
            selfSelector = o => o;
        }

        private void An_unary_selector()
        {
            unarySelector = o => Convert.ToDecimal(o);
        }

        private void We_try_to_ignore_on_that_self_selector()
        {
            var test = new CharacterizationTest();
            thrownException = Record.Exception(() => test.IgnoreOnType(selfSelector));
        }

        private void We_try_to_ignore_on_that_unary_selector()
        {
            var test = new CharacterizationTest();
            thrownException = Record.Exception(() => test.IgnoreOnType(unarySelector));
        }

        private void We_get_an_exception_saying_it_is_not_a_valid_member_expression()
        {
            Assert.IsType(typeof(ArgumentException), thrownException);
            Assert.Contains("is not a valid member expression", thrownException.Message);
        }

        private void A_lambda_representation_of_a_property_we_wish_to_ignore()
        {
            propertySelector = o => o.MyBoolean;
        }

        private void We_use_that_lambda_representation_when_we_compare_two_objects_that_differ_on_that_property()
        {
            var someObject = new ObjectWithValueTypeProperties();
            var anotherObject = new ObjectWithValueTypeProperties { MyBoolean = true };
            var test = new CharacterizationTest();
            test.IgnoreOnType(propertySelector);
            test.CompareObjects(someObject, anotherObject);
        }

        private void We_compare_two_lists_with_two_objects_each_that_differ_in_two_properties_each()
        {
            thrownException = Record.Exception(() => Compare_two_lists_with_two_objects_each_that_differ_in_two_properties_each());
        }

        private void Compare_two_lists_with_two_objects_each_that_differ_in_two_properties_each()
        {            
            var aList = new List<ObjectWithValueTypeProperties>
                            {
                                new ObjectWithValueTypeProperties(),
                                new ObjectWithValueTypeProperties {MyBoolean = true, MyInteger = 1}
                            };
            var anotherList = new List<ObjectWithValueTypeProperties>
                                  {
                                      new ObjectWithValueTypeProperties {MyByte = 1, MyChar = 'a'},
                                      new ObjectWithValueTypeProperties()
                                  };

            new CharacterizationTest().CompareObjects(aList, anotherList);
        }

        private void The_exception_message_contains_all_four_differences()
        {
            exceptionMessage = thrownException.Message;
            Assert.Contains("ObjectValuesDoNotMatch: LeftNodeName=IEnumerable0.MyByte", exceptionMessage);
            Assert.Contains("ObjectValuesDoNotMatch: LeftNodeName=IEnumerable0.MyChar", exceptionMessage);
            Assert.Contains("ObjectValuesDoNotMatch: LeftNodeName=IEnumerable1.MyBoolean", exceptionMessage);
            Assert.Contains("ObjectValuesDoNotMatch: LeftNodeName=IEnumerable1.MyInteger", exceptionMessage);
        }

        private void We_compare_two_lists_with_equal_objects_but_different_capacity()
        {
            var aList = new List<int>(1) {1};
            var anotherList = new List<int>(2) {1};
            new CharacterizationTest().CompareObjects(aList, anotherList);
        }

        private void We_compare_two_objects_with_circular_references()
        {
            var someObject = new ObjectWithSelfReference();
            new CharacterizationTest().CompareObjects(someObject, someObject);
        }

        private void We_have_mismatch_exception_for_a_specific_comparison()
        {
            thrownException = Record.Exception(() => Compare_two_objects_with_value_type_properties());
        }

        private void Compare_two_objects_with_value_type_properties()
        {
            var someObject = new ObjectWithValueTypeProperties();
            var anotherObject = new ObjectWithValueTypeProperties { MyBoolean = true };
            new CharacterizationTest().CompareObjects(someObject, anotherObject);
        }

        private void We_read_the_message_of_the_exception()
        {
            exceptionMessage = thrownException.Message;
        }

        private void It_tells_us_what_the_difference_is()
        {
            Assert.Contains("ObjectValuesDoNotMatch: LeftNodeName=ObjectWithValueTypeProperties.MyBoolean", exceptionMessage);
        }

        private void We_want_to_compare_two_objects()
        {
            // WTF? If I instantiate CharacterizationTest here, 
            // I *sometimes* get a null reference when I reference it later.
            //
            //test = new CharacterizationTest();
        }

        private void We_compare_true_and_true()
        {
            new CharacterizationTest().CompareObjects(true, true);
        }

        private void Nothing()
        {
        }

        private void Compare_true_and_false()
        {
            new CharacterizationTest().CompareObjects(true, false);
        }

        private void We_compare_true_and_false()
        {
            thrownException = Record.Exception(() => Compare_true_and_false());
        }

        private void We_get_the_appropriate_exception()
        {
            Assert.IsType(typeof(ObjectMismatchException), thrownException);
        }

        //private CharacterizationTest test;
        private Exception thrownException;
        private string exceptionMessage;
        private Expression<Func<ObjectWithValueTypeProperties, bool>> propertySelector;
        private Expression<Func<ObjectWithValueTypeProperties, ObjectWithValueTypeProperties>> selfSelector;
        private Expression<Func<ObjectWithValueTypeProperties, decimal>> unarySelector;
        private Expression<Func<ObjectWithMixedTypeProperties, ObjectWithValueTypeProperties>> referenceTypePropertySelector;
    }
    // ReSharper restore InconsistentNaming
    // ReSharper restore MemberCanBeMadeStatic.Local
}