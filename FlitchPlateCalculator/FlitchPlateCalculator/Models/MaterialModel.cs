namespace FlitchPlateCalculator.Models
{
    /// <summary>
    /// Enum for material types.
    /// </summary>
    public enum MaterialTypes
    {
        MATERIAL_UNDEFINED     = 0,
        MATERIAL_STEEL         = 1,
        MATERIAL_WOOD_SYP      = 2,
        MATERIAL_WOOD_DF       = 3,
        MATERIAL_WOOD_LVL_E2_0 = 4
    }
    public class MaterialModel
    {
        public MaterialTypes MaterialType { get; set; } = MaterialTypes.MATERIAL_UNDEFINED;

        // Young's Modulus - psi
        public int E { get; set; } = 0;

        // Material bending stress limit - psi
        public int Fb { get; set; } = 0;

        public MaterialModel(MaterialTypes type)
        {
            MaterialType = type;

            switch (type)
            {
                case MaterialTypes.MATERIAL_UNDEFINED:
                    {
                        throw new System.ArgumentException(type.ToString() + " error");
                    }
                case MaterialTypes.MATERIAL_STEEL:
                    {
                        E = 29000000;
                        Fb = 21600;
                        break;
                    }
                case MaterialTypes.MATERIAL_WOOD_SYP:
                    {
                        E = 2000000;
                        Fb = 2600;
                        break;
                    }
                case MaterialTypes.MATERIAL_WOOD_DF:
                    {
                        E = 2000000;
                        Fb = 2600;
                        break;
                    }
                case MaterialTypes.MATERIAL_WOOD_LVL_E2_0:
                    {
                        E = 2000000;
                        Fb = 2600;
                        break;
                    }
                default:
                    throw new System.ArgumentException("Error - unknown material type " + type.ToString());
            }
        }
    }


}
