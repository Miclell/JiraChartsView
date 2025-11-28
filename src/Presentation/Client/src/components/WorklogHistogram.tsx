import {useEffect, useState} from 'react';
import {Bar, BarChart, CartesianGrid, Legend, ResponsiveContainer, Tooltip, XAxis, YAxis} from 'recharts';
import {jiraApi} from '../services/api';
import type {WorklogHistogramDto} from '../types/api';

interface WorklogHistogramProps {
  projectKey: string;
}

export function WorklogHistogram({ projectKey }: WorklogHistogramProps) {
  const [data, setData] = useState<WorklogHistogramDto['histogram']>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        setError(null);
        const response = await jiraApi.getWorklogHistogram(projectKey);
        console.log('WorklogHistogram response:', response);
        setData(response.histogram || []);
      } catch (err) {
        console.error('WorklogHistogram error:', err);
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
      <div className="w-full h-96">
        <h3 className="text-xl font-semibold mb-4 text-gray-800">
          Гистограмма времени по залогированному времени (только закрытые задачи)
        </h3>
        <div className="flex items-center justify-center h-full">
          <div className="text-red-500">Ошибка: {error}</div>
        </div>
      </div>
    );
  }

  if (!data || data.length === 0 || data.every(item => item.taskCount === 0)) {
    return (
      <div className="w-full h-96">
        <h3 className="text-xl font-semibold mb-4 text-gray-800">
          Гистограмма времени по залогированному времени (только закрытые задачи)
        </h3>
        <div className="flex items-center justify-center h-full">
          <div className="text-gray-500">Нет данных для отображения.</div>
        </div>
      </div>
    );
  }

  return (
    <div className="w-full h-96">
      <h3 className="text-xl font-semibold mb-4 text-gray-800">
        Гистограмма времени жизни задач (от создания до закрытия, только закрытые задачи)
      </h3>
      <ResponsiveContainer width="100%" height="100%">
        <BarChart data={data.filter(item => item.taskCount > 0)} margin={{ top: 20, right: 30, left: 20, bottom: 100 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis 
            dataKey="timeRange" 
            angle={-45} 
            textAnchor="end" 
            height={100}
            tick={{ fontSize: 12 }}
          />
          <YAxis label={{ value: 'Количество задач', angle: -90, position: 'insideLeft' }} />
          <Tooltip />
          <Legend />
          <Bar dataKey="taskCount" fill="#3b82f6" name="Количество задач" />
        </BarChart>
      </ResponsiveContainer>
    </div>
  );
}

