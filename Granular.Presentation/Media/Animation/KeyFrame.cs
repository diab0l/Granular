using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Media.Animation
{
    public abstract class KeyFrame<T> : Freezable
    {
        public static readonly DependencyProperty ValueProperty = DependencyPropertyRegisterNonGeneric("Value", typeof(T), typeof(KeyFrame<T>), new FrameworkPropertyMetadata());
        public T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty KeyTimeProperty = DependencyPropertyRegisterNonGeneric("KeyTime", typeof(KeyTime), typeof(KeyFrame<T>), new FrameworkPropertyMetadata());
        public KeyTime KeyTime
        {
            get { return (KeyTime)GetValue(KeyTimeProperty); }
            set { SetValue(KeyTimeProperty, value); }
        }

        public abstract T InterpolateValue(T baseValue, double keyFrameProgress);

        protected static DependencyProperty DependencyPropertyRegisterNonGeneric(string name, Type propertyType, Type ownerType, PropertyMetadata metadata = null, ValidateValueCallback validateValueCallback = null)
        {
            return typeof(T).IsGenericParameter ? null : DependencyProperty.Register(name, propertyType, ownerType, metadata, validateValueCallback);
        }
    }

    public class DiscreteKeyFrame<T> : KeyFrame<T>
    {
        private IAnimationOperations<T> animationOperations;

        public DiscreteKeyFrame(IAnimationOperations<T> animationOperations)
        {
            this.animationOperations = animationOperations;
        }

        public override T InterpolateValue(T baseValue, double keyFrameProgress)
        {
            return keyFrameProgress < 1 ? baseValue : Value;
        }
    }

    public class LinearKeyFrame<T> : KeyFrame<T>
    {
        private IAnimationOperations<T> animationOperations;

        public LinearKeyFrame(IAnimationOperations<T> animationOperations)
        {
            this.animationOperations = animationOperations;
        }

        public override T InterpolateValue(T baseValue, double keyFrameProgress)
        {
            return animationOperations.Interpolate(baseValue, Value, keyFrameProgress);
        }
    }

    public class EasingKeyFrame<T> : KeyFrame<T>
    {
        public static readonly DependencyProperty EasingFunctionProperty = DependencyPropertyRegisterNonGeneric("EasingFunction", typeof(IEasingFunction), typeof(EasingKeyFrame<T>), new FrameworkPropertyMetadata());
        public IEasingFunction EasingFunction
        {
            get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
            set { SetValue(EasingFunctionProperty, value); }
        }

        private IAnimationOperations<T> animationOperations;

        public EasingKeyFrame(IAnimationOperations<T> animationOperations)
        {
            this.animationOperations = animationOperations;
        }

        public override T InterpolateValue(T baseValue, double keyFrameProgress)
        {
            if (EasingFunction != null)
            {
                keyFrameProgress = EasingFunction.Ease(keyFrameProgress);
            }

            return animationOperations.Interpolate(baseValue, Value, keyFrameProgress);
        }
    }
}
