package com.example.inspectxr.repository;

import com.example.inspectxr.entity.PaintAnnotationEntity;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface PaintAnnotationRepository extends JpaRepository<PaintAnnotationEntity, Long> {
    List<PaintAnnotationEntity> findAllBySessionId(Long sessionId);
}
