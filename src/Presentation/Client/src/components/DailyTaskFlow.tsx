import {useEffect, useState} from 'react';
import {CartesianGrid, Legend, Line, LineChart, ResponsiveContainer, Tooltip, XAxis, YAxis} from 'recharts';
import {jiraApi} from '../services/api';
import type {DailyTaskFlowDto} from '../types/api';

interface DailyTaskFlowProps {
  projectKey: string;
}

export function DailyTaskFlow({ projectKey }: DailyTaskFlowProps) {
  const [data, setData] = useState<DailyTaskFlowDto['dailyFlow']>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const response = await jiraApi.getDailyTaskFlow(projectKey);
        setData(response.dailyFlow);
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

  // Calculate cumulative totals
  let cumulativeCreated = 0;
  let cumulativeResolved = 0;
  
  const chartData = data.map(item => {
    cumulativeCreated += item.createdCount;
    cumulativeResolved += item.resolvedCount;
    return {
      date: new Date(item.date).toLocaleDateString('ru-RU', { day: '2-digit', month: '2-digit' }),
      created: item.createdCount,
      resolved: item.resolvedCount,
      cumulativeCreated,
      cumulativeResolved,
    };
  });

  return (
    <div className="w-full h-96">
      <h3 className="text-xl font-semibold mb-4 text-gray-800">Ежедневный поток задач</h3>
      <ResponsiveContainer width="100%" height="100%">
        <LineChart data={chartData} margin={{ top: 20, right: 30, left: 20, bottom: 60 }}>
          <CartesianGrid strokeDasharray="3 3" />
          <XAxis 
            dataKey="date" 
            angle={-45}
            textAnchor="end"
            height={80}
            tick={{ fontSize: 12 }}
          />
          <YAxis />
          <Tooltip />
          <Legend />
          <Line 
            type="monotone" 
            dataKey="created" 
            stroke="#3b82f6" 
            name="Создано (день)" 
            strokeWidth={2}
          />
          <Line 
            type="monotone" 
            dataKey="resolved" 
            stroke="#10b981" 
            name="Решено (день)" 
            strokeWidth={2}
          />
          <Line 
            type="monotone" 
            dataKey="cumulativeCreated" 
            stroke="#60a5fa" 
            name="Создано (накопительный итог)" 
            strokeWidth={2}
            strokeDasharray="5 5"
          />
          <Line 
            type="monotone" 
            dataKey="cumulativeResolved" 
            stroke="#34d399" 
            name="Решено (накопительный итог)" 
            strokeWidth={2}
            strokeDasharray="5 5"
          />
        </LineChart>
      </ResponsiveContainer>
    </div>
  );
}

