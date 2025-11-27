package com.example.inspectxr.repository;

import com.example.inspectxr.entity.PointAnnotationEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface PointAnnotationRepository extends JpaRepository<PointAnnotationEntity, Long> {
    List<PointAnnotationEntity> findAllBySessionId(Long sessionId);
}
