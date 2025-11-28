import {useEffect, useState} from 'react';
import {Bar, BarChart, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis} from 'recharts';
import {jiraApi} from '../services/api';

interface OpenTimeHistogramProps {
  projectKey: string;
}

interface HistogramData {
  range: string;
  count: number;
}

export function OpenTimeHistogram({ projectKey }: OpenTimeHistogramProps) {
  const [data, setData] = useState<HistogramData[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const response = await jiraApi.getOpenTimeHistogram(projectKey);
        
        // Calculate open time for each issue (only closed tasks)
        const openTimes: number[] = response.issues
          .map(issue => {
            const created = issue.fields?.created ? new Date(issue.fields.created).getTime() : null;
            const resolved = issue.fields?.resolutiondate ? new Date(issue.fields.resolutiondate).getTime() : null;
            // Only count closed tasks (must have resolutiondate)
            if (created && resolved) {
              return (resolved - created) / (1000 * 60 * 60 * 24); // days
            }
            return null;
          })
          .filter((time): time is number => time !== null);

        // Create histogram bins (0-7, 7-14, 14-30, 30-60, 60-90, 90+ days)
        const bins = [0, 7, 14, 30, 60, 90, Infinity];
        const histogram: HistogramData[] = bins.slice(0, -1).map((start, index) => {
          const end = bins[index + 1];
          const count = openTimes.filter(time => time >= start && time < end).length;
          const range = end === Infinity ? `${start}+ дней` : `${start}-${end} дней`;
          return { range, count };
        });

        setData(histogram);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Ошибка загрузки данных');
      } finally {
        setLoading(false);
      }
    };

    if (projectKey) {
      fetchData();
    }
  }, [projectKey]);

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-gray-500">Загрузка...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-red-500">Ошибка: {error}</div>
      </div>
    );
  }

  return (
    <div className="w-full h-96">
      <h3 className="text-xl font-semibold mb-4 text-gray-800">Гистограмма времени открытия задач</h3>
      <ResponsiveContainer width="100%" height="100%">
        <BarChart data={data} margin={{ top: 20, right: 30, left: 20, bottom: 20 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis 
            dataKey="range" 
            tick={{ fontSize: 12 }}
            angle={-45}
            textAnchor="end"
            height={80}
          />
          <YAxis />
          <Tooltip />
          <Legend />
          <Bar dataKey="count" fill="#3b82f6" name="Количество задач" />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}

