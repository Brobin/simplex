using RaikesSimplexService.DuckTheSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using RaikesSimplexService.DataModel;
using RaikesSimplexService.DuckTheSystem;
using MathNet.Numerics.LinearAlgebra;

namespace UnitTests
{
    
    
    /// <summary>
    ///This is a test class for SolverTest and is intended
    ///to contain all SolverTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SolverTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Solve
        ///</summary>
        [TestMethod]
        public void ExampleSolveTest()
        {
            #region Arrange

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { 8, 12 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 24
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 12, 12 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 36
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[2] { 2, 1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 4
            };

            var lc4 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 5
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2, lc3, lc4 };

            var goal = new Goal()
            {
                Coefficients = new double[2] { 0.2, 0.3 },
                ConstantTerm = 0
            };

            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Minimize
            };


            var expected = new Solution()
            {
                Decisions = new double[2] { 3, 0 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 0.6
            };
            #endregion

            //Act

            var solver = new Solver();
            var actual = solver.Solve(model);

            #region Print lines
            System.Diagnostics.Debug.WriteLine(model.DuckString(true));
            System.Diagnostics.Debug.WriteLine(solver.DuckString());
            System.Diagnostics.Debug.WriteLine(actual.DuckString());
            #endregion

            //Assert
            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }

        [TestMethod]
        public void TwoPhaseTest1()
        {
            var constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 1, 1, 0 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 1
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 2, -1, 0 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 1
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0, 3, 0 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 2
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 1, 0, 0 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 0
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0, 1, 0 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 0
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0, 0, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 2
            });
            var goal = new Goal()
            {
                Coefficients = new double[] { 6, 3, 1 },
                ConstantTerm = 0
            };
            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Maximize
            };
            var expected = new Solution()
            {
                Decisions = new double[3] { 1, 0, 2 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 8
            };
            var solver = new Solver();
            var actual = solver.Solve(model);

            #region Print lines
            System.Diagnostics.Debug.WriteLine(model.DuckString(true));
            System.Diagnostics.Debug.WriteLine(solver.DuckString());
            System.Diagnostics.Debug.WriteLine(actual.DuckString());
            #endregion

            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }

        [TestMethod]
        public void TwoPhaseTest2()
        {
            var constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 1, 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 40
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 2, 1, -1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 10
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0, -1, 1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 10
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 1, 0, 0 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 0
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0, 1, 0 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 0
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0, 0, 1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 0
            });
            var goal = new Goal()
            {
                Coefficients = new double[] { 2, 3, 1 },
                ConstantTerm = 0
            };
            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Maximize
            };
            var expected = new Solution()
            {
                Decisions = new double[3] { 10, 10, 20 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 70
            };
            var solver = new Solver();
            var actual = solver.Solve(model);

            #region Print lines
            System.Diagnostics.Debug.WriteLine(model.DuckString(true));
            System.Diagnostics.Debug.WriteLine(solver.DuckString());
            System.Diagnostics.Debug.WriteLine(actual.DuckString());
            #endregion

            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }

        [TestMethod]
        public void TwoPhaseTest3()
        {
            var constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 1, 1, 0 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 27
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 1, 0, 0 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 10
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0, 1, 0 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 20
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0, 1, 1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 25
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 0, 0, 1 },
                Relationship = Relationship.GreaterThanOrEquals,
                Value = 1
            });
            var goal = new Goal()
            {
                Coefficients = new double[] { 1, 2, 3 },
                ConstantTerm = 0
            };
            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Maximize
            };
            var expected = new Solution()
            {
                Decisions = new double[3] { 10, 20, 5 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 65
            };
            var solver = new Solver();
            var actual = solver.Solve(model);

            #region Print lines
            System.Diagnostics.Debug.WriteLine(model.DuckString(true));
            System.Diagnostics.Debug.WriteLine(solver.DuckString());
            System.Diagnostics.Debug.WriteLine(actual.DuckString());
            #endregion

            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }

        [TestMethod]
        public void TwoPhaseEqualsTest()
        {
            var constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 10, 5 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 50
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 6, 6 },
                Relationship = Relationship.LessThanOrEquals,
                //Relationship = Relationship.Equals,
                Value = 36
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 4.5, 18 },
                //Relationship = Relationship.LessThanOrEquals,
                Relationship = Relationship.Equals,
                Value = 81
            });
            var goal = new Goal()
            {
                Coefficients = new double[] { 9, 7 },
                ConstantTerm = 0
            };
            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Maximize
            };
            var expected = new Solution()
            {
                Decisions = new double[2] { 2, 4 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 46
            };
            var solver = new Solver();
            var actual = solver.Solve(model);

            #region Print lines
            System.Diagnostics.Debug.WriteLine(model.DuckString(true));
            System.Diagnostics.Debug.WriteLine(solver.DuckString());
            System.Diagnostics.Debug.WriteLine(actual.DuckString());
            #endregion

            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }

        [TestMethod]
        public void SinglePhaseEqualsTest()
        {
            var constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 10, 5 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 50
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 6, 6 },
                Relationship = Relationship.LessThanOrEquals,
                //Relationship = Relationship.Equals,
                Value = 36
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 4.5, 18 },
                //Relationship = Relationship.LessThanOrEquals,
                Relationship = Relationship.Equals,
                Value = 81
            });
            var goal = new Goal()
            {
                Coefficients = new double[] { 9, 7 },
                ConstantTerm = 0
            };
            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Maximize
            };
            var expected = new Solution()
            {
                Decisions = new double[2] { 2, 4 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 46
            };
            var solver = new Solver();
            var actual = solver.Solve(model);

            #region Print lines
            System.Diagnostics.Debug.WriteLine(model.DuckString(true));
            System.Diagnostics.Debug.WriteLine(solver.DuckString());
            System.Diagnostics.Debug.WriteLine(actual.DuckString());
            #endregion

            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }

        [TestMethod]
        public void SinglePhaseNormalTest()
        {
            var constraints = new List<LinearConstraint>();
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 35
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 1, 2 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 38
            });
            constraints.Add(new LinearConstraint()
            {
                Coefficients = new double[] { 2, 2 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 50
            });
            var goal = new Goal()
            {
                Coefficients = new double[] { 350, 450 },
                ConstantTerm = 0
            };
            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Maximize
            };
            var expected = new Solution()
            {
                Decisions = new double[2] { 12, 13 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 10050
            };
            var solver = new Solver();
            var actual = solver.Solve(model);

            #region Print lines
            System.Diagnostics.Debug.WriteLine(model.DuckString(true));
            System.Diagnostics.Debug.WriteLine(solver.DuckString());
            System.Diagnostics.Debug.WriteLine(actual.DuckString());
            #endregion

            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }
       
        [TestMethod]
        public void SinglePhaseMinimizeTest()
        {

            #region Test2
            var target = new Solver();

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { 2, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 32
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 18
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 3 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 36
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2, lc3 };

            var goal = new Goal()
            {
                Coefficients = new double[2] { 80, 70 },
                ConstantTerm = 0
            };

            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Minimize
            };

            var expected = new Solution()
            {
                Decisions = new double[2] { 14, 4 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 1400
            };
            #endregion

            //Act
            var actual = target.Solve(model);

            //Assert
            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }
        
        [TestMethod]
        public void SinglePhaseMinimizeTest2()
        {
            #region Test3
            var target = new Solver();

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 35
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 2 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 38
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[2] { 2, 2 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 50
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2, lc3 };

            var goal = new Goal()
            {
                Coefficients = new double[2] { 350, 450 },
                ConstantTerm = 0
            };

            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Minimize
            };

            var expected = new Solution()
            {
                Decisions = new double[2] { 12, 13 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 10050
            };
            #endregion

            //Act
            var actual = target.Solve(model);

            //Assert
            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }

        [TestMethod()]
        public void SinglePhaseMinimizeTest3()
        {
            #region Test3
            var target = new Solver();

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 4 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 24
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 2 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 16
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2 };

            var goal = new Goal()
            {
                Coefficients = new double[2] { 3, 9 },
                ConstantTerm = 0
            };

            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Minimize
            };

            var expected = new Solution()
            {
                Decisions = new double[2] { 8, 4 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 60
            };
            #endregion

            //Act
            var actual = target.Solve(model);

            //Assert
            CollectionAssert.AreEqual(expected.Decisions, actual.Decisions);
            Assert.AreEqual(expected.Quality, actual.Quality);
            Assert.AreEqual(expected.AlternateSolutionsExist, actual.AlternateSolutionsExist);
        }

        [TestMethod()]
        public void SinglePhaseMinimizeTest4()
        {
            #region Arrange
            var target = new Solver();

            var lc1 = new LinearConstraint()
            {
                Coefficients = new double[2] { -1, 2 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 36
            };

            var lc2 = new LinearConstraint()
            {
                Coefficients = new double[2] { 1, 6 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 132
            };

            var lc3 = new LinearConstraint()
            {
                Coefficients = new double[2] { 3, 5 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 136
            };

            var lc4 = new LinearConstraint()
            {
                Coefficients = new double[2] { 5, 3 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 136
            };

            var lc5 = new LinearConstraint()
            {
                Coefficients = new double[2] { 6, 1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 132
            };

            var lc6 = new LinearConstraint()
            {
                Coefficients = new double[2] { 2, -1 },
                Relationship = Relationship.LessThanOrEquals,
                Value = 36
            };

            var constraints = new List<LinearConstraint>() { lc1, lc2, lc3, lc4, lc5, lc6 };

            var goal = new Goal()
            {
                Coefficients = new double[2] { 10, 10 },
                ConstantTerm = 0
            };

            var model = new Model()
            {
                Constraints = constraints,
                Goal = goal,
                GoalKind = GoalKind.Minimize
            };

            var expected = new Solution()
            {
                Decisions = new double[2] { 17, 17 },
                Quality = SolutionQuality.Optimal,
                AlternateSolutionsExist = false,
                OptimalValue = 340
            };
        }
            #endregion
    }


}