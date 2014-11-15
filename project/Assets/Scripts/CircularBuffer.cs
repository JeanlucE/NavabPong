public class CircularBuffer {
	private float[] values;
	private int index = 0;
	private int size;
	private int count = 0;

	public CircularBuffer(int size){
		values = new float[size];
		this.size = size;
	}

	public void Add(float v){
		values [index++] = v;
		index %= size;

		if (count < size)
			count++;
	}

	public float Average(){
		float result = 0f;

		for (int i = 0; i < size; i++)
			result += values [i];

		return (count == 0) ? 0 : result / (float)count;
	}

}
