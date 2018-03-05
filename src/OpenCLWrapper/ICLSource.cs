namespace OpenCLWrapper
{
    public interface ICLSource
    {
        // CL Methods
        int get_global_id(int val);

        int min(int val1, int val2);
        float min(float val1, float val2);

        int max(int val1, int val2);
        float max(float val1, float val2);

        float sqrt(float val1);
    }
}
